namespace AspNetCoreBoilerplate.Shared.Entities;

public interface ISoftDeletableEntity
{
    bool IsDeleted { get; }
    Guid? DeletedById { get; }
    DateTime? DeletedAt { get; }
    void SoftDelete(Guid? deletedById);
}

public class SoftDeletableEntity<TKey> : DomainEntity<TKey>, ISoftDeletableEntity
{
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

public class SoftDeletableEntity : SoftDeletableEntity<Guid>;

