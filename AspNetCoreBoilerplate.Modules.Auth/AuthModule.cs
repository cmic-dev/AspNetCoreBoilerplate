using AspNetCoreBoilerplate.Modules.Auth.Application.Abstractions;
using AspNetCoreBoilerplate.Modules.Auth.Application.EventHandlers;
using AspNetCoreBoilerplate.Modules.Auth.Core.Events;
using AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Jwt;
using AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Providers;
using AspNetCoreBoilerplate.Modules.Auth.Infrastructure.Services;
using AspNetCoreBoilerplate.Shared;
using AspNetCoreBoilerplate.Shared.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AspNetCoreBoilerplate.Modules.Auth;

public class AuthModule : ModuleBase
{
    public override string Name => "Auth";

    public override Assembly Assembly => typeof(AuthModule).Assembly;

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer();

        services.Configure<JwtOptions>(configuration
            .GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordProvider, PasswordProvider>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IProfileService, ProfileService>();

        services.AddScoped<IDomainEventHandler<UserCreatedEvent>, CreateUserProfileHandler>();
    }
}

