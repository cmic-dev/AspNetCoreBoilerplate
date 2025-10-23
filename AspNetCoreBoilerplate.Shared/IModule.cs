using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AspNetCoreBoilerplate.Shared;

public interface IModule
{
    string Name { get; }

    Assembly Assembly { get; }

    Version Version { get; }

    void ConfigureServices(IServiceCollection services, IConfiguration configuration);

    void Configure(WebApplication app);

    Task InitializeAsync(IServiceProvider serviceProvider);

    Task ShutdownAsync();
}
