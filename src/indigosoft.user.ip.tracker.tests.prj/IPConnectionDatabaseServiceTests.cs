using Indigosoft.User.Ip.Tracker.Database;
using Indigosoft.User.Ip.Tracker.Database.Dal;
using Indigosoft.User.Ip.Tracker.Database.Services;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Indigosoft.User.Ip.Tracker.Tests.Services;

public class IPConnectionDatabaseServiceTests
{
    private readonly Mock<IApplicationDbProvider> _mockDbProvider = new();
    private readonly Mock<ApplicationContext> _mockContext = new();
    private readonly IPConnectionDatabaseService _service;
    private readonly EventQueue<IPConnectionRequest> _eventQueue = new();

    public IPConnectionDatabaseServiceTests()
    {
        _service = new IPConnectionDatabaseService(_mockDbProvider.Object, _eventQueue);
    }

    [Fact]
    public async Task GetUserIpsAsync_ReturnsDistinctIps()
    {
        // Arrange
        var data = new List<UserIpConnection>
        {
            new() { UserId = 1, IpAddress = "1.1.1.1" },
            new() { UserId = 1, IpAddress = "2.2.2.2" },
            new() { UserId = 1, IpAddress = "1.1.1.1" },
            new() { UserId = 2, IpAddress = "3.3.3.3" }
        };

        var mockSet = data.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(c => c.UserIpConnections).Returns(mockSet.Object);
        _mockDbProvider.Setup(p => p.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_mockContext.Object);

        // Act
        var result = await _service.GetUserIpsAsync(1);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("1.1.1.1", result);
        Assert.Contains("2.2.2.2", result);
    }

    [Fact]
    public async Task GetLastConnectionAsync_ReturnsLast()
    {
        // Arrange
        var date = DateTime.UtcNow;
        var guid = Guid.NewGuid();
        var data = new List<UserIpConnection>
        {
            new() { Id = guid, UserId = 5, IpAddress = "1.1.1.1", Updated = date.AddMinutes(-10) },
            new() { Id = guid, UserId = 5, IpAddress = "2.2.2.2", Updated = date }
        };

        var expected = data[1];

        var mockSet = data.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.UserIpConnections).Returns(mockSet.Object);
        _mockDbProvider.Setup(p => p.GetAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_mockContext.Object);

        // Act
        var result = await _service.GetLastConnectionAsync(5);

        // Assert
        Assert.Equal(expected.Id, result.Id);
        Assert.Equal(expected.UserId, result.UserId);
        Assert.Equal(expected.IpAddress, result.IpAddress);
        Assert.Equal(expected.Updated, result.Updated);
    }
}
