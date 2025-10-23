namespace AspNetCoreBoilerplate.Shared.Entities;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
    string? DeletedBy { get; }
    DateTime? DeletedAt { get; }
    string? DeletedByIp { get; }
    string? DeletedByDevice { get; }

    void SoftDelete(string? deletedBy, string? deletedByIp = null, string? deletedByDevice = null);
    void Restore();
}

public class SoftDeletableEntity<TKey> : DomainEntity<TKey>, ISoftDeletableEntity
{
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

public class SoftDeletableEntity : SoftDeletableEntity<Guid>;
