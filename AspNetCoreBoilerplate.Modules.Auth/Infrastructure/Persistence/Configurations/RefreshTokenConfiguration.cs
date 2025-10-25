using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens", Constants.AUTH_DB_SCHEMA);

        builder.HasIndex(rt => rt.Token)
            .IsUnique();

        builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked });

        builder.HasIndex(rt => rt.ExpiryTime);

        builder.HasIndex(rt => new { rt.UserId, rt.ExpiryTime, rt.IsRevoked });

        // Relationships
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
