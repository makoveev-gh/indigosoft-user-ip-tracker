using Indigosoft.User.Ip.Tracker.Database.Dal;
using Indigosoft.User.Ip.Tracker.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Indigosoft.User.Ip.Tracker.Database;

public class ApplicationContext : DbContext
{
    #region Statics

    /// <summary>
    /// Returns the default database connection string.
    /// </summary>
    public static string DefaultConnectionString { get; }

    #endregion

    #region Properties

    public virtual DbSet<Dal.User> Users => Set<Dal.User>();
    public virtual DbSet<UserIpConnection> UserIpConnections => Set<UserIpConnection>();
    public virtual DbSet<LogEntry> LogEntrys => Set<LogEntry>();

    #endregion

    #region Data

    private readonly string _connectionString;

    #endregion

    #region Constructors

    /// <summary>
    /// Static constructor to initialize the default database path.
    /// </summary>
    static ApplicationContext()
    {
        Directory.CreateDirectory(ProfileLocationStorage.DatabaseDirPath);
        var defaultDbPath = Path.Combine(
            ProfileLocationStorage.DatabaseDirPath,
            "indigosoft.db");

        DefaultConnectionString = $"Data Source={defaultDbPath}";
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationContext"/> with the default connection string.
    /// </summary>
    public ApplicationContext()
    {
        _connectionString = DefaultConnectionString;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="ApplicationContext"/> with specific options.
    /// </summary>
    /// <param name="options">Options for configuring the database context.</param>
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    #endregion

    #region Methods

    /// <inheritdoc/>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(optionsBuilder is null) throw new ArgumentNullException(nameof(optionsBuilder));

        base.OnConfiguring(optionsBuilder);

        if(optionsBuilder.IsConfigured) return;

        optionsBuilder.UseNpgsql(_connectionString ?? DefaultConnectionString);
    }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        if(modelBuilder is null) throw new ArgumentNullException(nameof(modelBuilder));

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(Dal.User.GetConfiguration());
        modelBuilder.ApplyConfiguration(UserIpConnection.GetConfiguration());
        modelBuilder.ApplyConfiguration(LogEntry.GetConfiguration());
    }
    #endregion
}
