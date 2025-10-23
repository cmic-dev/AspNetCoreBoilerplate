using AspNetCoreBoilerplate.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreBoilerplate.Core;

public class ModuleOrchestrator : IModuleOrchestrator
{
    private readonly List<IModule> _modules = new();
    private readonly ILogger<ModuleOrchestrator>? _logger;
    private bool _isInitialized;

    public IReadOnlyList<IModule> Modules => _modules.AsReadOnly();

    public ModuleOrchestrator(ILogger<ModuleOrchestrator>? logger = null)
    {
        _logger = logger;
    }

    public IModuleOrchestrator AddModule(IModule module)
    {
        if (module == null)
            throw new ArgumentNullException(nameof(module));

        if (_modules.Any(m => m.Name == module.Name))
            throw new InvalidOperationException($"Module '{module.Name}' is already registered.");

        _modules.Add(module);
        _logger?.LogInformation("Registered module: {ModuleName}", module.Name);
        return this;
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        _logger?.LogInformation("Configuring services for {Count} modules", _modules.Count);

        foreach (var module in _modules)
        {
            try
            {
                _logger?.LogDebug("Configuring services for module: {ModuleName}", module.Name);
                module.ConfigureServices(services, configuration);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error configuring services for module: {ModuleName}", module.Name);
                throw;
            }
        }
    }

    public void Configure(WebApplication app)
    {
        _logger?.LogInformation("Configuring application pipeline for {Count} modules", _modules.Count);

        foreach (var module in _modules)
        {
            try
            {
                _logger?.LogDebug("Configuring pipeline for module: {ModuleName}", module.Name);
                module.Configure(app);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error configuring pipeline for module: {ModuleName}", module.Name);
                throw;
            }
        }
    }

    public async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        if (_isInitialized)
            throw new InvalidOperationException("Modules have already been initialized.");

        _logger?.LogInformation("Initializing {Count} modules", _modules.Count);

        foreach (var module in _modules)
        {
            try
            {
                _logger?.LogDebug("Initializing module: {ModuleName}", module.Name);
                await module.InitializeAsync(serviceProvider);
                _logger?.LogInformation("Module initialized successfully: {ModuleName}", module.Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error initializing module: {ModuleName}", module.Name);
                throw;
            }
        }

        _isInitialized = true;
        _logger?.LogInformation("All modules initialized successfully");
    }

    public async Task ShutdownAsync()
    {
        if (!_isInitialized)
        {
            _logger?.LogWarning("Shutdown called but modules were not initialized");
            return;
        }

        _logger?.LogInformation("Shutting down {Count} modules", _modules.Count);

        foreach (var module in _modules)
        {
            try
            {
                _logger?.LogDebug("Shutting down module: {ModuleName}", module.Name);
                await module.ShutdownAsync();
                _logger?.LogInformation("Module shut down successfully: {ModuleName}", module.Name);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error shutting down module: {ModuleName}", module.Name);
            }
        }

        _isInitialized = false;
        _logger?.LogInformation("All modules shut down");
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
