using AspNetCoreBoilerplate.Core.ExceptionHandlers;
using AspNetCoreBoilerplate.Core.HealthChecks;
using AspNetCoreBoilerplate.Core.Infrastructure.Persistence;
using AspNetCoreBoilerplate.Core.Infrastructure.Persistence.Interceptors;
using AspNetCoreBoilerplate.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AspNetCoreBoilerplate.Core;

public class CoreModule : ModuleBase
{
    public override string Name => nameof(CoreModule);
    public override Assembly Assembly => typeof(CoreModule).Assembly;
    public override Version Version => new Version(1, 0, 0);

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<DomainEventDispatcherInterceptor>();
        services.AddScoped<EntityAuditInterceptor>();

        // EF core 
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<IAppDbContext, AppDbContext>(options =>
            options.UseSqlServer(connectionString, builder =>
                builder.MigrationsAssembly(typeof(AppDbContext).Assembly)));

        // Health Checks
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: ["db", "sql"], timeout: TimeSpan.FromSeconds(5));

        // Configure global exception handling 
        services.AddExceptionHandler<RateLimitExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        // Services
        services.AddScoped<IUserContext, UserContext>();
    }

    public override void Configure(WebApplication app)
    {
        app.MapGet("/", () => "The application is running");
    }

    public override async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<CoreModule>>();
        try
        {
            var scope = serviceProvider.CreateScope();
            IAppDbContext dbContext = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            await dbContext.Database.MigrateAsync();

            logger.LogInformation("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
