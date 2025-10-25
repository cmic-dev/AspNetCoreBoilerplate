using AspNetCoreBoilerplate.Shared.Events;

namespace AspNetCoreBoilerplate.Modules.Auth.Core.Events;

public class UserCreatedEvent : DomainEvent
{
    public required Guid UserId { get; init; }
    public required string UserName { get; init; }
}
