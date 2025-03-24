using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database.Services;

/// <summary>
/// Interface for working with user IP connections.
/// </summary>
public interface IIPConnectionDatabaseService
{
    /// <summary>
    /// Inserts a new IP connection or updates the timestamp if it already exists.
    /// </summary>
    /// <param name="connection">The user IP connection to insert or update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddConnectionInQueue(IPConnectionRequest connection, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of all distinct IP addresses used by the specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of IP addresses.</returns>
    Task<List<string>> GetUserIpsAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the last recorded connection for the specified user.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The last <see cref="IPConnectionResponse"/>, or null if none found.</returns>
    Task<IPConnectionResponse> GetLastConnectionAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all user IDs who have used IP addresses that contain the specified fragment.
    /// </summary>
    /// <param name="ipFragment">Part of the IP address to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of matching user IDs.</returns>
    Task<List<long>> GetUsersByIpFragmentAsync(string ipFragment, CancellationToken cancellationToken = default);
}
