using AspNetCoreBoilerplate.Shared.Abstractions;

namespace AspNetCoreBoilerplate.Shared.Events;

public interface IDomainEventHandler<TEvent> where TEvent : IDomainEvent
{
    public Task HandleAsync(TEvent domainEvent, IUserContext userContext, CancellationToken ctn = default);
}
