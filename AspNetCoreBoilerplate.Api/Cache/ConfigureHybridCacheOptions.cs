using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace AspNetCoreBoilerplate.Api.Cache;

public class ConfigureHybridCacheOptions : IConfigureOptions<HybridCacheOptions>
{
    public void Configure(HybridCacheOptions options)
    {
        options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5),
            LocalCacheExpiration = TimeSpan.FromMinutes(1)
        };
    }
}
