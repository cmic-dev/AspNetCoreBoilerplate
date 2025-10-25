using AspNetCoreBoilerplate.Core.Infrastructure.Orchestrators;
using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AspNetCoreBoilerplate.Core.Extensions;

public static class CoreExtensions
{
    public static WebApplicationBuilder ConfigureModules(this WebApplicationBuilder builder, Action<ModuleOrchestrator> orchestratorOption)
    {
        var orchestrator = new ModuleOrchestrator();
        orchestratorOption?.Invoke(orchestrator);
        orchestrator.ConfigureServices(builder.Services, builder.Configuration);
        builder.Services.AddSingleton<IModuleOrchestrator>(orchestrator);
        return builder;
    }

    public static async Task InitializeModulesAndRunAsync(this WebApplication app)
    {
        IModuleOrchestrator moduleOrchestrator = app.Services
            .GetRequiredService<IModuleOrchestrator>();

        moduleOrchestrator.Configure(app);

        IHostApplicationLifetime lifetime = app.Services
            .GetRequiredService<IHostApplicationLifetime>();

        lifetime.ApplicationStopping.Register(async () =>
            await moduleOrchestrator.ShutdownAsync());

        await moduleOrchestrator.InitializeAsync(app.Services);

        await app.RunAsync();
    }
}


