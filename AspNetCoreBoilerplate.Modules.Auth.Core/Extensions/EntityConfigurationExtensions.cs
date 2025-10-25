using AspNetCoreBoilerplate.Shared.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Extensions;

public static class EntityConfigurationExtensions
{
    public static EntityTypeBuilder<T> ConfigureAuditableEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, IAuditableEntity
    {
        builder.HasIndex(e => e.CreatedById);
        builder.HasIndex(e => e.UpdatedById);

        return builder;
    }

    public static EntityTypeBuilder<T> ConfigureDeletableEntity<T>(this EntityTypeBuilder<T> builder)
        where T : class, ISoftDeletableEntity
    {
        builder.HasIndex(e => e.DeletedById);
        builder.HasQueryFilter(e => !e.IsDeleted);

        return builder;
    }
}
