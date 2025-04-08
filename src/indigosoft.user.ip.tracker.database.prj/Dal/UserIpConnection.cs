using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Net;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Indigosoft.User.Ip.Tracker.Database.Dal;

public class UserIpConnection
{
	#region Helpers

	private sealed class Configuration : IEntityTypeConfiguration<UserIpConnection>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<UserIpConnection> builder)
		{
			if(builder is null) throw new ArgumentNullException(nameof(builder));

			builder.ToTable("user_ip_connections");

			builder
				.Property(e => e.Id)
				.HasColumnName("id")
				.HasColumnType("uuid")
				.ValueGeneratedOnAdd()
				.IsRequired();

			builder
				.Property(e => e.UserId)
				.HasColumnName("user_id")
                .HasColumnType("bigint")
                .IsRequired();

            builder
                .Property(e => e.IpAddress)
                .HasColumnName("ip_address")
                .HasColumnType("inet")
				.HasConversion(
					v => IPAddress.Parse(v),
					v => v.ToString()
				)
                .IsRequired();

            builder
                .Property(e => e.Created)
				.HasColumnName("created")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder
                .Property(e => e.Updated)
                .HasColumnName("updated")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.IpAddress })
                .IsUnique();

            builder
				.HasOne<User>()
				.WithMany()
				.HasForeignKey(e => e.UserId)
				.OnDelete(DeleteBehavior.Cascade);
        }
	}

	#endregion

	#region Statics

	public static IEntityTypeConfiguration<UserIpConnection> GetConfiguration() => new Configuration();

	#endregion

	#region Properties

    public Guid Id { get; set; }

    public long UserId { get; set; }

    public string IpAddress { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }

    #endregion

    #region ctor
    public UserIpConnection()
	{
	}

	#endregion
}
