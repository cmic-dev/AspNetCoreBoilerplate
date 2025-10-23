using AspNetCoreBoilerplate.Modules.Auth.Core.Entities;
using AspNetCoreBoilerplate.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Infrastructure.EntityConfigurations;

internal class LoginHistoryConfiguration : IEntityTypeConfiguration<LoginHistory>
{
    public void Configure(EntityTypeBuilder<LoginHistory> builder)
    {
        builder.ToTable("LoginHistories", Constants.AUTH_DB_SCHEMA);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Timestamp);

        builder.HasIndex(e => new
        {
            e.UserId,
            e.Timestamp
        });
    }
}
