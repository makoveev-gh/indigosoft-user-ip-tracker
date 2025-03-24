using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database;

/// <inheritdoc cref="IApplicationDbProvider"/>
/// <summary>
/// Provides instances of <see cref="ApplicationContext"/> and ensures database initialization.
/// </summary>
/// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
public class ApplicationDbProvider(IServiceProvider serviceProvider) : IApplicationDbProvider
{
    private static readonly ILogger Log = Serilog.Log.ForContext<ApplicationDbProvider>();
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly SemaphoreSlim _locker = new(1, 1);

    /// <summary>
    /// Indicates whether the database has been initialized.
    /// </summary>
    public bool IsInit { get; private set; }

    /// <summary>
    /// Returns an instance of <see cref="ApplicationContext"/> and ensures that the database is initialized.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The initialized database context.</returns>
    public async ValueTask<ApplicationContext> GetAsync(CancellationToken cancellationToken = default)
    {
        var serviceScope = _serviceProvider.CreateAsyncScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationContext>();
        if(IsInit) return dbContext;

        await LockAsync(async () =>
        {
            if(!IsInit)
            {
                Log.Information("Begin database initialization");

                await dbContext.Database.MigrateAsync(cancellationToken);

                Log.Information("Database initialized");
                IsInit = true;
            }
        }, cancellationToken);

        return dbContext;
    }

    /// <summary>
    /// Executes the specified asynchronous action within a thread-safe lock.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="token">Cancellation token.</param>
    private async Task LockAsync(Func<Task> action, CancellationToken token)
    {
        await _locker.WaitAsync(token);
        try
        {
            await action();
        }
        finally
        {
            _locker.Release();
        }
    }
}
