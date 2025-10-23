using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class FluxorConfigurationExtensions
{
    public static IServiceCollection AddFluxorConfiguration(this IServiceCollection services)
    {
        services.AddFluxor(options =>
        {
            options.ScanAssemblies(typeof(FluxorConfigurationExtensions).Assembly);
            options.UseReduxDevTools(rdt =>
            {
                rdt.Name = AppConstants.APP_NAME;
                rdt.EnableStackTrace();
            });
        });
        return services;
    }
}
