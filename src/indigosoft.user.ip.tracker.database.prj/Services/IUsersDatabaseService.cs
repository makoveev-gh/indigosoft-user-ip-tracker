using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database.Services;

public interface IUsersDatabaseService
{
    /// <summary>
    /// Retrieves a list of all users from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users.</returns>
    public Task<List<UserData>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new user into the database.
    /// </summary>
    /// <param name="user">User to insert.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous insert operation.</returns>
    public Task InsertUserAsync(UserData user, CancellationToken cancellationToken = default);
}
