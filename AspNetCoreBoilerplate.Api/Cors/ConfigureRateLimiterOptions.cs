using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace AspNetCoreBoilerplate.Api.Cors;

public class ConfigureCorsOptions(IConfiguration configuration) : IConfigureOptions<CorsOptions>
{
    public void Configure(CorsOptions options)
    {
        var allowedOrigins = configuration.GetSection("AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        options.AddPolicy("CorsPolicy", builder =>
        {
            builder.WithOrigins(allowedOrigins)
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });

        options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("*")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }
}
