using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles", Constants.AUTH_DB_SCHEMA);

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.ProfilePictureUrl)
            .HasMaxLength(100);

        builder.Property(u => u.Gender)
            .HasMaxLength(20);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.HasIndex(up => up.UserId).IsUnique();

        builder.HasOne(up => up.User)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
