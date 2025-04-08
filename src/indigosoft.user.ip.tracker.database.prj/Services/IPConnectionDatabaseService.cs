using Indigosoft.User.Ip.Tracker.Infrastructure;
using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Indigosoft.User.Ip.Tracker.Database.Services;

/// <summary>
/// Service for managing user IP connections in the database.
/// </summary>
public class IPConnectionDatabaseService(IApplicationDbProvider applicationDbProvider, EventQueue<IPConnectionRequest> eventQueue) : IIPConnectionDatabaseService
{
    #region Data
    private static readonly ILogger Log = Serilog.Log.ForContext<IPConnectionDatabaseService>();
    private readonly IApplicationDbProvider _applicationDbProvider = applicationDbProvider;
    private readonly EventQueue<IPConnectionRequest> _eventQueue = eventQueue;

    #endregion

    /// <inheritdoc/>
    public async Task AddConnectionInQueue(IPConnectionRequest connection, CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));
        if(connection is null) throw new ArgumentNullException(nameof(connection));
        if(!IPAddress.TryParse(connection.IpAddress, out _)) throw new InvalidIpAddressFormatException(nameof(connection));

        try
        {
            _eventQueue.Enqueue(connection);
            Log.Information("The object has been added to the queue");
        }
        catch(Exception ex)
        {
            Log.Information(ex.Message);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetUserIpsAsync(long userId, CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));
        var context = await _applicationDbProvider.GetAsync(cancellationToken);

        Log.Information("Fetching IP addresses for userId: {UserId}", userId);
        var result =  await context.UserIpConnections
            .Where(x => x.UserId == userId)
            .Select(x => x.IpAddress.ToString())
            .Distinct()
            .ToListAsync(cancellationToken);

        Log.Information("Fetched {Count} IP addresses for userId: {UserId}", result.Count, userId);
        return result;
    }

    /// <inheritdoc/>
    public async Task<IPConnectionResponse> GetLastConnectionAsync(long userId, CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));
        var context = await _applicationDbProvider.GetAsync(cancellationToken);

        Log.Information("Fetching last IP connection for userId: {UserId}", userId);
        Serilog.Log.Information("Log to DB test at {Time}", DateTime.UtcNow);
        var connections = await context.UserIpConnections
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Updated)
            .FirstOrDefaultAsync(cancellationToken);

        if(connections == null)
        {
            Log.Warning("No IP connection found for userId: {UserId}", userId);
            return null!;
        }
        Log.Information("Last IP connection for userId: {UserId} is {Ip} at {Updated}", userId, connections.IpAddress, connections.Updated);

        return new IPConnectionResponse
        {
            Id = connections.Id,
            Updated = connections.Updated,
            IpAddress = connections.IpAddress.ToString(),
            UserId = userId,
        };
    }

    /// <inheritdoc/>
    public async Task<List<long>> GetUsersByIpFragmentAsync(string ipFragment, CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));
        if(string.IsNullOrWhiteSpace(ipFragment)) throw new ArgumentException("IP fragment must not be null or empty", nameof(ipFragment));

        Log.Information("Searching users by IP fragment: {IpFragment}", ipFragment);

        var context = await _applicationDbProvider.GetAsync(cancellationToken);

        var result = await context.UserIpConnections
            .Where(x => EF.Functions.ILike(x.IpAddress, $"%{ipFragment}%"))
            .Select(x => x.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        Log.Information("Found {Count} unique users for IP fragment: {IpFragment}", result.Count, ipFragment);

        return result;
    }
}
