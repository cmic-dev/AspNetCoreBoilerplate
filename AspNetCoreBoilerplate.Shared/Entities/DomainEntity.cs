using AspNetCoreBoilerplate.Shared.Events;

namespace AspNetCoreBoilerplate.Shared.Entities;

public interface IDomainEntity
{
    IReadOnlyList<IDomainEvent> DomainEvents();
}

public class DomainEntity<TKey> : IDomainEntity
{
    private IList<IDomainEvent> domainEvents = new List<IDomainEvent>();

    public TKey Id { get; protected set; }

    public IReadOnlyList<IDomainEvent> DomainEvents() =>
        domainEvents.ToList().AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent) =>
        domainEvents.Add(domainEvent);
}
