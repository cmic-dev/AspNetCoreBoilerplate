using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreBoilerplate.Shared;

public interface IModuleOrchestrator
{
    IReadOnlyList<IModule> Modules { get; }
    IModuleOrchestrator AddModule(IModule module);
    void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    void Configure(WebApplication app);
    Task InitializeAsync(IServiceProvider serviceProvider);
    Task ShutdownAsync();
    IModule? GetModule(string name);
    T? GetModule<T>() where T : IModule;
}
