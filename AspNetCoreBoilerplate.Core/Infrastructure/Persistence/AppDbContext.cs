using AspNetCoreBoilerplate.Core.Infrastructure.Persistence.Interceptors;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Persistence;

internal class AppDbContext(EntityAuditInterceptor auditIcp,
    DomainEventDispatcherInterceptor eventDipatcherIcp,
    IModuleOrchestrator orchestrator, DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(auditIcp);
        optionsBuilder.AddInterceptors(eventDipatcherIcp);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var module in orchestrator.Modules)
        {
            var configs = module.Assembly
                .GetTypes()
                .Where(t =>
                    !t.IsAbstract &&
                    !t.IsGenericTypeDefinition &&
                    t.GetInterfaces().Any(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
                .ToList();

            foreach (var config in configs)
            {
                dynamic? instance = Activator.CreateInstance(config);
                if (instance != null)
                {
                    modelBuilder.ApplyConfiguration(instance);
                }
            }
        }
    }
}
