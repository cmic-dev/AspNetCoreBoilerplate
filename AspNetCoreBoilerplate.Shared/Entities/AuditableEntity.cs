namespace AspNetCoreBoilerplate.Shared.Entities;

public interface IAuditableEntity
{
    Guid? CreatedById { get; }
    DateTime CreatedAt { get; }
    void RecordCreatedBy(Guid? createdById);

    Guid? UpdatedById { get; }
    DateTime? UpdatedAt { get; }
    void RecordUpdatedBy(Guid? updatedById);
}

public class AuditableEntity<TKey> : DomainEntity<TKey>, IAuditableEntity
{
    public Guid? CreatedById { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Guid? UpdatedById { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public void RecordCreatedBy(Guid? createdById)
    {
        CreatedById = createdById;
        CreatedAt = DateTime.UtcNow;
    }

    public void RecordUpdatedBy(Guid? updatedById)
    {
        UpdatedById = updatedById;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class AuditableEntity : AuditableEntity<Guid>;
