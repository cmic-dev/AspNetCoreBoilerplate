using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", Constants.AUTH_DB_SCHEMA);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(u => new { u.UserName, u.IsDeleted })
            .IsUnique()
            .HasFilter("[IsDeleted] = CAST(0 AS bit)");

        builder.HasData([
            User.System,
            User.SuperAdmin
        ]);
    }
}
