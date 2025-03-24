using Indigosoft.User.Ip.Tracker.Database;
using Indigosoft.User.Ip.Tracker.Database.Services;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Indigosoft.User.Ip.Tracker.Tests.Services;

public class UsersDatabaseServiceTests
{
    private readonly Mock<IApplicationDbProvider> _mockDbProvider = new();
    private readonly Mock<ApplicationContext> _mockContext = new();
    private readonly UsersDatabaseService _service;

    public UsersDatabaseServiceTests()
    {
        _service = new UsersDatabaseService(_mockDbProvider.Object);
    }

    [Fact]
    public async Task GetAllUsersAsyncTest()
    {
        // Arrange
        var users = new List<Database.Dal.User> {
            new() { Id = 1, Name = "Admin" },
            new() { Id = 2, Name = "NotAdmin" }
        };

        var mockSet = users.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Users).Returns(mockSet.Object);
        _mockDbProvider.Setup(p => p.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockContext.Object);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Admin", result[0].Name);
        Assert.Equal("NotAdmin", result[1].Name);
    }

    [Fact]
    public async Task InsertUserAsyncTest()
    {
        // Arrange
        var userDto = new UserData { Id = 3, Name = "Admin" };
        var users = new List<Database.Dal.User>();

        var mockSet = users.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.Users).Returns(mockSet.Object);
        _mockDbProvider.Setup(p => p.GetAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockContext.Object);

        // Act
        await _service.InsertUserAsync(userDto);

        // Assert
        _mockContext.Verify(c => c.Users.AddAsync(It.Is<Database.Dal.User>(u => u.Id == 3 && u.Name == "Admin"), It.IsAny<CancellationToken>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
