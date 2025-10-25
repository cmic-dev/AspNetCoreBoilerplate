using AspNetCoreBoilerplate.Shared.Entities;
using AspNetCoreBoilerplate.Shared.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Persistence.Interceptors;

public class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly List<IDomainEvent> _domainEvents = [];

    public DomainEventDispatcherInterceptor(IDomainEventDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        CaptureDomainEvents(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        CaptureDomainEvents(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        var savedChangesResult = base.SavedChanges(eventData, result);
        try
        {
            var eventsToDispatch = _domainEvents
                .OrderBy(x => x.CreatedAt)
                .Where(e => !e.IsDispatched)
                .ToList();

            if (!eventsToDispatch.Any())
                return savedChangesResult;

            _dispatcher.DispatchAsync(eventsToDispatch).GetAwaiter().GetResult();
        }
        finally
        {
            _domainEvents.Clear();
            ClearDomainEventsFromEntities(eventData.Context);
        }
        return savedChangesResult;
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var savedChangesResult = await base.SavedChangesAsync(eventData, result, cancellationToken);
        try
        {
            var eventsToDispatch = _domainEvents
                .OrderBy(x => x.CreatedAt)
                .Where(e => !e.IsDispatched)
                .ToList();

            if (!eventsToDispatch.Any())
                return savedChangesResult;

            await _dispatcher.DispatchAsync(eventsToDispatch, cancellationToken);
        }
        finally
        {
            _domainEvents.Clear();
            ClearDomainEventsFromEntities(eventData.Context);
        }
        return savedChangesResult;
    }

    private void CaptureDomainEvents(DbContext? context)
    {
        _domainEvents.Clear();
        if (context == null)
            return;

        var domainEvents = context.ChangeTracker.Entries<IDomainEntity>()
            .SelectMany(e => e.Entity.DomainEvents())
            .ToList();
        _domainEvents.AddRange(domainEvents);
    }

    private static void ClearDomainEventsFromEntities(DbContext? context)
    {
        if (context == null)
            return;

        var entities = context.ChangeTracker.Entries<IDomainEntity>()
            .Select(e => e.Entity)
            .ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }
    }
}
