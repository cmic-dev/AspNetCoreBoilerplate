using AspNetCoreBoilerplate.Shared.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreBoilerplate.Core.Infrastructure.Orchestrators;

public class ModuleOrchestrator : IModuleOrchestrator
{
    private readonly List<IModule> _modules = new();
    public IReadOnlyList<IModule> Modules => _modules.AsReadOnly();

    public IModuleOrchestrator AddModule(IModule module)
    {
        if (module == null)
            throw new ArgumentNullException(nameof(module));

        if (_modules.Any(m => m.Name == module.Name))
            throw new InvalidOperationException($"Module '{module.Name}' is already registered.");

        _modules.Add(module);
        return this;
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        foreach (var module in _modules)
        {
            try
            {
                module.ConfigureServices(services, configuration);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public void Configure(WebApplication app)
    {
        foreach (var module in _modules)
        {
            try
            {
                module.Configure(app);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        foreach (var module in _modules)
        {
            try
            {
                await module.InitializeAsync(serviceProvider);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public async Task ShutdownAsync()
    {
        foreach (var module in _modules)
        {
            try
            {
                await module.ShutdownAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error shutting down module '{module.Name}': {ex.Message}");
            }
        }
    }

    public IModule? GetModule(string name)
    {
        return _modules.FirstOrDefault(m => m.Name == name);
    }

    public T? GetModule<T>() where T : IModule
    {
        return _modules.OfType<T>().FirstOrDefault();
    }
}
