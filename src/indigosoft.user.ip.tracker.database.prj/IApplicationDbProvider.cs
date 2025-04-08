using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database;

/// <summary>
/// Provides access to the application database context.
/// </summary>
public interface IApplicationDbProvider
{
    /// <summary>
    /// Asynchronously retrieves an instance of <see cref="ApplicationContext"/>.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that returns an instance of <see cref="ApplicationContext"/>.</returns>
    ValueTask<ApplicationContext> GetAsync(CancellationToken cancellationToken = default);
}
