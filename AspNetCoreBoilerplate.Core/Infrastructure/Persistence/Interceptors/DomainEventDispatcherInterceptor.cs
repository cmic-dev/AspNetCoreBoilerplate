using AspNetCoreBoilerplate.Shared.Entities;
using AspNetCoreBoilerplate.Shared.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Persistence.Interceptors;

public class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly List<IDomainEvent> _domainEvents = [];

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
        DispatchCapturedEvents().GetAwaiter().GetResult(); // Ensure sync
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        await DispatchCapturedEvents(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public void CaptureDomainEvents(DbContext? context)
    {
        _domainEvents.Clear();

        if (context == null)
            return;

        var domainEvents = context.ChangeTracker.Entries<IDomainEntity>()
            .SelectMany(e => e.Entity.DomainEvents())
            .ToList();

        _domainEvents.AddRange(domainEvents);
    }

    private async Task DispatchCapturedEvents(CancellationToken cancellationToken = default)
    {
        if (_domainEvents.Count == 0)
            return;

        var tasks = _domainEvents.Select(async domainEvent =>
        {
            // Dispatch the domain event (e.g., publish to a message broker)
            await Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }
}
