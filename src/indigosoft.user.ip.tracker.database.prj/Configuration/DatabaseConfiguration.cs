using System.Runtime.Serialization;

namespace Indigosoft.User.Ip.Tracker.Database.Configuration;

[DataContract]
public class DatabaseConfiguration
{
    /// <summary>
    /// Gets or sets the connection parameters for the PostgreSQL database.
    /// </summary>
    /// <value>
    /// Connection parameters for PostgreSQL.
    /// </value>
    [DataMember]
    public PostgreSqlConnectionStringParameters PostgreSql { get; set; } = new();
}
