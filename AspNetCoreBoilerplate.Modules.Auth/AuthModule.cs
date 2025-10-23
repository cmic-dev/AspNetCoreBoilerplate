using AspNetCoreBoilerplate.Modules.Auth.Application.Providers;
using AspNetCoreBoilerplate.Modules.Auth.Application.Services;
using AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Jwt;
using AspNetCoreBoilerplate.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AspNetCoreBoilerplate.Modules.Auth;

public class AuthModule : ModuleBase
{
    public override string Name => nameof(AuthModule);
    public override Assembly Assembly => typeof(AuthModule).Assembly;
    public override Version Version => new Version(1, 0, 0);

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Options
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        // Authentication scheme
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme);

        // Config
        services.Configure<JwtOptions>(configuration
            .GetSection(JwtOptions.SectionName));

        // Providers
        services.AddScoped<JwtProvider>();
        services.AddScoped<PasswordProvider>();

        // Services
        services.AddScoped<AuthService>();
        services.AddScoped<ProfileService>();
    }
}
