using AspNetCoreBoilerplate.Web.Core.Localization;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Services;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class MudBlazorConfigurationExtensions
{
    public static IServiceCollection AddMudBlazorConfiguration(this IServiceCollection services)
    {
        // MudBlazor Translation
        services.AddScoped<MudLocalizer, StringLocalizationProvider>();
        services.AddLocalizationInterceptor<LocalizationInterceptor>();

        // MudBlazor Services
        services.AddMudPopoverService();
        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PreventDuplicates = true;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 5000;
            config.SnackbarConfiguration.HideTransitionDuration = 200;
            config.SnackbarConfiguration.ShowTransitionDuration = 200;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            config.SnackbarConfiguration.BackgroundBlurred = true;
        });
        return services;
    }
}
