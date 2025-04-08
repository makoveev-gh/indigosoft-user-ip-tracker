using System;
using System.Runtime.Serialization;

namespace Indigosoft.User.Ip.Tracker.Database.Configuration;

[DataContract]
public record PostgreSqlConnectionStringParameters
{
    /// <summary>
    /// Gets or sets the host where the database server is located.
    /// </summary>
    [DataMember]
    public string Host { get; init; } = "localhost";

    /// <summary>
    /// Gets or sets the name of the database.
    /// </summary>
    [DataMember]
    public string DatabaseName { get; init; } = "indigosoft";

    /// <summary>
    /// Gets or sets the port on which the database server is running.
    /// </summary>
    [DataMember]
    public int Port { get; init; } = 5432;

    /// <summary>
    /// Gets or sets the username used to connect to the database.
    /// </summary>
    [DataMember]
    public string Username { get; init; } = "postgres";

    /// <summary>
    /// Gets or sets the password used to connect to the database.
    /// </summary>
    [DataMember]
    public string Password { get; init; } = "admin";

    /// <summary>
    /// Gets or sets the connection timeout to the database.
    /// </summary>
    [DataMember]
    public TimeSpan ConnectTimeout { get; init; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Gets or sets the timeout for executing a command.
    /// </summary>
    [DataMember]
    public TimeSpan CommandTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the timeout for asynchronous command cancellation.
    /// </summary>
    [DataMember]
    public TimeSpan CancellationTimeout { get; init; } = TimeSpan.FromSeconds(5);
}
