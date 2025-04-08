using Indigosoft.User.Ip.Tracker.Infrastructure.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database.Services;

public class UsersDatabaseService(IApplicationDbProvider applicationDbProvider) : IUsersDatabaseService
{
	#region Data

	private static readonly Serilog.ILogger Log = Serilog.Log.ForContext<UsersDatabaseService>();
	private readonly IApplicationDbProvider _applicationDbProvider = applicationDbProvider;

    #endregion

    /// <inheritdoc/>
    public async Task<List<UserData>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));

        Log.Information("Fetching all users from the database...");
        var result = new List<UserData>();
        var context = await _applicationDbProvider.GetAsync(cancellationToken);
        var users = await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach(var user in users) 
            result.Add(new UserData { Id = user.Id, Name = user.Name });

        Log.Information("Fetched {UserCount} users", users.Count);
        return result;
    }

    /// <inheritdoc/>
    public async Task InsertUserAsync(UserData user, CancellationToken cancellationToken = default)
    {
        if(_applicationDbProvider is null) throw new ArgumentNullException(nameof(_applicationDbProvider));
        if(user is null) throw new ArgumentNullException(nameof(user));

        Log.Information("Inserting new user into the database: {@User}", user);

        var context = await _applicationDbProvider.GetAsync(cancellationToken);

        var dal = new Dal.User { Name = user.Name, Id = user.Id };

        await context.Users.AddAsync(dal, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        Log.Information("User inserted successfully: {@User}", user);
    }
}
