namespace AspNetCoreBoilerplate.Shared.Entities;

public interface IAuditableEntity
{
    string? CreatedBy { get; }
    string? CreatedByIp { get; }
    string? CreatedByDevice { get; }
    DateTime? CreatedAt { get; }

    string? UpdatedBy { get; }
    DateTime? UpdatedAt { get; }
    string? UpdatedByIp { get; }
    string? UpdatedByDevice { get; }

    void RecordCreatedBy(string? createdBy, string? createdByIp = null, string? createdByDevice = null);
    void RecordUpdatedBy(string? updatedBy, string? updatedByIp = null, string? updatedByDevice = null);
}

public class AuditableEntity<TKey> : DomainEntity<TKey>, IAuditableEntity
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
}

public class AuditableEntity : AuditableEntity<Guid>;
