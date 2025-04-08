using System;
using System.Threading;
using System.Threading.Tasks;

namespace Indigosoft.User.Ip.Tracker.Database;

/// <summary>
/// Initializes the database on application startup.
/// </summary>
public class DatabaseInitializer : IInitializable
{
    #region Data

    private readonly IApplicationDbProvider _applicationDbProvider;

    #endregion

    #region Properties

    /// <summary>
    /// Indicates whether the database has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Creates an instance of <see cref="DatabaseInitializer"/>.
    /// </summary>
    /// <param name="applicationDbProvider">The database provider.</param>
    public DatabaseInitializer(IApplicationDbProvider applicationDbProvider)
    {
        _applicationDbProvider = applicationDbProvider ?? throw new ArgumentNullException(nameof(applicationDbProvider));
    }

    #endregion

    #region Implementation of IInitializable

    /// <inheritdoc/>
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        using var dbContext = await _applicationDbProvider.GetAsync(cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        IsInitialized = true;
    }

    #endregion
}
