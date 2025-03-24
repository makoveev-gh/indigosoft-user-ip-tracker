using Npgsql;

namespace Indigosoft.User.Ip.Tracker.Database.Configuration;

/// <summary>Provides extension methods for <see cref="PostgreSqlConnectionStringParameters"/>.</summary>
public static class PostgreSqlConnectionStringParametersExtensions
{
    /// <summary>Converts the specified <paramref name="parameters"/> into a PostgreSQL connection string.</summary>
    /// <param name="parameters">PostgreSQL connection parameters.</param>
    /// <returns>A PostgreSQL connection string.</returns>
    public static string ToConnectionString(this PostgreSqlConnectionStringParameters parameters)
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Database = parameters.DatabaseName,
            Host = parameters.Host,
            Port = parameters.Port,
            Username = parameters.Username,
            Password = parameters.Password,
            Timeout = (int)parameters.ConnectTimeout.TotalSeconds,
            CommandTimeout = (int)parameters.CommandTimeout.TotalSeconds,
            CancellationTimeout = (int)parameters.CancellationTimeout.TotalMilliseconds,
            IncludeErrorDetail = true
        };

        return connectionStringBuilder.ConnectionString;
    }
}
