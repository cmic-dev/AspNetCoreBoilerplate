using AspNetCoreBoilerplate.Web.Core.DelegatingHandlers;
using AspNetCoreBoilerplate.Web.Services;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class ApiServiceConfigurationExtensions
{
    public static IServiceCollection AddApiServiceConfiguration(this IServiceCollection services, string apiUrl)
    {
        services.AddScoped<AuthStorageService>();

        services.AddScoped<PublicApiDelegatingHandler>();

        services.AddHttpClient<PublicApiService>(options =>
            options.BaseAddress = new Uri(apiUrl))
            .AddHttpMessageHandler<PublicApiDelegatingHandler>();

        services.AddHttpClient<PrivateApiService>(options =>
            options.BaseAddress = new Uri(apiUrl));

        services.AddHttpClient<AuthApiService>(options =>
            options.BaseAddress = new Uri(apiUrl));

        services.AddScoped<ProfileService>();

        return services;
    }
}
