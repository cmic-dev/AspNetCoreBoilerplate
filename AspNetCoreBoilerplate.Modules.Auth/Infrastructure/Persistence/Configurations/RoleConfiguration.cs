using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", Constants.AUTH_DB_SCHEMA);

        builder.HasData([
            Role.SuperAdmin,
            Role.Admin,
            Role.Moderator,
            Role.Manager,
            Role.Editor,
            Role.User,
            Role.Guest,
        ]);
    }
}
