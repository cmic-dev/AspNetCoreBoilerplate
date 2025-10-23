namespace AspNetCoreBoilerplate.Shared.Entities;

public class EntityBase<TKey> : DomainEntity<TKey>, IAuditableEntity, ISoftDeletableEntity
{
    public string? CreatedBy { get; private set; }
    public string? CreatedByIp { get; private set; }
    public string? CreatedByDevice { get; private set; }
    public DateTime? CreatedAt { get; private set; }

    public string? UpdatedBy { get; private set; }
    public string? UpdatedByIp { get; private set; }
    public string? UpdatedByDevice { get; private set; }
    public DateTime? UpdatedAt { get; private set; }


    public void RecordCreatedBy(string? createdBy, string? createdByIp = null, string? createdByDevice = null)
    {
        CreatedBy = createdBy;
        CreatedByIp = createdByIp;
        CreatedByDevice = createdByDevice;
        CreatedAt = DateTime.UtcNow;
    }

    public void RecordUpdatedBy(string? updatedBy, string? updatedByIp = null, string? updatedByDevice = null)
    {
        UpdatedBy = updatedBy;
        UpdatedByIp = updatedByIp;
        UpdatedByDevice = updatedByDevice;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsDeleted { get; private set; } = false;
    public string? DeletedBy { get; private set; }
    public string? DeletedByIp { get; private set; }
    public string? DeletedByDevice { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public void SoftDelete(string? deletedBy, string? deletedByIp = null, string? deletedByDevice = null)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        DeletedByIp = deletedByIp;
        DeletedByDevice = deletedByDevice;
        DeletedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedByIp = null;
        DeletedByDevice = null;
        DeletedAt = null;
    }
}

public class EntityBase : EntityBase<Guid>;