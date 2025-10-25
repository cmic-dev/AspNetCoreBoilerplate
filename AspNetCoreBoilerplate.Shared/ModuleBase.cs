using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AspNetCoreBoilerplate.Shared;

public abstract class ModuleBase : IModule
{
    public abstract string Name { get; }
    public abstract Assembly Assembly { get; }

    public virtual void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddApplicationPart(Assembly);
    }

    public virtual void Configure(WebApplication app) { }

    public virtual Task InitializeAsync(IServiceProvider serviceProvider)
    {
        return Task.CompletedTask;
    }

    public virtual Task ShutdownAsync()
    {
        return Task.CompletedTask;
    }
}
