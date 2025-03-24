using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Indigosoft.User.Ip.Tracker.Database.Dal;
public class User
{
	#region Helpers

	private sealed class Configuration : IEntityTypeConfiguration<User>
	{
		/// <inheritdoc />
		public void Configure(EntityTypeBuilder<User> builder)
		{
			if(builder is null) throw new ArgumentNullException(nameof(builder));

			builder.ToTable("users");

			builder
				.Property(e => e.Id)
				.HasColumnName("id")
                .HasColumnType("bigint")
                .IsRequired();

			builder
				.Property(e => e.Name)
				.HasColumnName("name")
                .HasColumnType("varchar")
                .IsRequired();
		}
	}

	#endregion

	#region Statics
	public static IEntityTypeConfiguration<User> GetConfiguration() => new Configuration();

	#endregion

	#region Properties

	public required long Id { get; set; }

	public required string Name { get; set; }

	#endregion

	#region ctor

	public User()
	{
	}

	#endregion
}
