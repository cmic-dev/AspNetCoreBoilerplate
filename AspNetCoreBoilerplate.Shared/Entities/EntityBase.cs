
namespace AspNetCoreBoilerplate.Shared.Entities;

public class EntityBase<TKey> : DomainEntity<TKey>, IAuditableEntity, ISoftDeletableEntity
{
    public Guid? CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public void RecordCreatedBy(Guid? createdById)
    {
        CreatedById = createdById;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid? UpdatedById { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public void RecordUpdatedBy(Guid? updatedById)
    {
        UpdatedById = updatedById;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDeleted { get; private set; }
    public Guid? DeletedById { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public void SoftDelete(Guid? deletedById)
    {
        IsDeleted = true;
        DeletedById = deletedById;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedById = null;
        DeletedAt = null;
    }
}
