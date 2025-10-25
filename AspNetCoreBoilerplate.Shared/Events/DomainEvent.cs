namespace AspNetCoreBoilerplate.Shared.Events;

public interface IDomainEvent
{
    bool IsDispatched { get; }
    DateTime? DispatchedAt { get; }
    DateTime CreatedAt { get; }

    void MarkAsDispatched(); 
}

public class DomainEvent : IDomainEvent
{
    public DomainEvent()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public DateTime CreatedAt { get; private set; }
    public bool IsDispatched { get; private set; }
    public DateTime? DispatchedAt { get; private set; }

    public void MarkAsDispatched()
    {
        IsDispatched = true;
        DispatchedAt = DateTime.UtcNow;
    } 
}
