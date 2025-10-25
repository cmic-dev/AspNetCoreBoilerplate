using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Persistence.Interceptors;

public class EntityAuditInterceptor(IUserContext userContext) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetAuditMetadata(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetAuditMetadata(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void SetAuditMetadata(DbContext? context)
    {
        if (context == null) return;

        var userId = userContext.UserId;

        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                       e.State == EntityState.Modified ||
                       e.State == EntityState.Deleted);

        foreach (var entry in entries)
        {
            // Handle auditable entities
            if (entry.Entity is IAuditableEntity auditableEntity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.RecordCreatedBy(userId);
                        break;

                    case EntityState.Modified:
                        auditableEntity.RecordUpdatedBy(userId);
                        break;
                }
            }

            // Handle soft delete
            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDeletableEntity softDeletable)
            {
                entry.State = EntityState.Modified;
                softDeletable.SoftDelete(userId);
            }
        }
    }
}
