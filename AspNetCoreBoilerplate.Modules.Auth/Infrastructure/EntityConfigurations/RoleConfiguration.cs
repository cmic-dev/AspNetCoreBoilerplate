using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.EntityConfigurations;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles", Constants.AUTH_DB_SCHEMA);

        builder.HasData([
            Role.SuperAdminRole,
            Role.AdminRole,
            Role.ModeratorRole,
            Role.ManagerRole,
            Role.EditorRole,
            Role.UserRole,
            Role.GuestRole,
        ]);
    }
}
