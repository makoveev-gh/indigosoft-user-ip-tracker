using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;

namespace Indigosoft.User.Ip.Tracker.Database.Dal;

public class LogEntry
{
    private sealed class Configuration : IEntityTypeConfiguration<LogEntry>
    {
        public void Configure(EntityTypeBuilder<LogEntry> builder)
        {
            builder.ToTable("logs");

            builder
                .Property(e => e.Id)
                .HasColumnName("id")
                .UseIdentityAlwaysColumn()
                .IsRequired();

            builder
                .Property(e => e.Timestamp)
                .HasColumnName("timestamp")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder
                .Property(e => e.Level)
                .HasColumnName("level")
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(e => e.Message)
                .HasColumnName("message")
                .HasColumnType("text")
                .IsRequired();

            builder
                .Property(e => e.Exception)
                .HasColumnName("exception")
                .HasColumnType("text");

            builder
                .Property(e => e.Properties)
                .HasColumnName("properties")
                .HasColumnType("jsonb")
                .IsRequired();
        }
    }

    #region Statics
    public static IEntityTypeConfiguration<LogEntry> GetConfiguration() => new Configuration();

    #endregion

    #region Properties
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = null!;
    public string Message { get; set; } = null!;
    public string Exception { get; set; }
    public string Properties { get; set; } = "{}";

    #endregion

    #region ctor

    public LogEntry()
    {
    }

    #endregion

}
