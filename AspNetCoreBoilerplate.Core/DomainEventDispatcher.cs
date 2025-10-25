using AspNetCoreBoilerplate.Shared.Abstractions;
using AspNetCoreBoilerplate.Shared.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreBoilerplate.Core;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IUserContext _userContext;
    private readonly ILogger<DomainEventDispatcher> _logger;

    public DomainEventDispatcher(IServiceScopeFactory scopeFactory, ILogger<DomainEventDispatcher> logger, IUserContext userContext)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);

        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

        using var scope = _scopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetServices(handlerType);

        var tasks = new List<Task>();

        foreach (var handler in handlers)
        {
            var handleMethod = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync));
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, _userContext, cancellationToken })!;
                tasks.Add(task);
            }
        }

        try
        {
            await Task.WhenAll(tasks);
            _logger.LogInformation("Successfully dispatched domain event: {EventType}", domainEvent.GetType().Name);
            domainEvent.MarkAsDispatched();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error dispatching domain event: {EventType}", domainEvent.GetType().Name);
            throw;
        }
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        var eventList = domainEvents.ToList();

        if (eventList.Count == 0)
        {
            return;
        }

        _logger.LogInformation("Dispatching {Count} domain events", eventList.Count);

        var tasks = eventList.Select(e => DispatchAsync(e, cancellationToken));
        await Task.WhenAll(tasks);
    }
}
