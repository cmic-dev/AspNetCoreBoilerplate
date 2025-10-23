using AspNetCoreBoilerplate.Web.Core.Authentication;

namespace AspNetCoreBoilerplate.Web.Core.Extensions;

public static class AddAuhtorizationConfiguration
{
    public static IServiceCollection AddAuthorizationCoreConfiguration(this IServiceCollection services)
    {
        services.AddAuthorizationCore(options =>
        {
            // Individual role policies
            options.AddPolicy(Policies.RequireSuperAdminRole, policy =>
                policy.RequireRole(Roles.SuperAdmin));

            options.AddPolicy(Policies.RequireAdminRole, policy =>
                policy.RequireRole(Roles.Admin));

            options.AddPolicy(Policies.RequireModeratorRole, policy =>
                policy.RequireRole(Roles.Moderator));

            options.AddPolicy(Policies.RequireManagerRole, policy =>
                policy.RequireRole(Roles.Manager));

            options.AddPolicy(Policies.RequireEditorRole, policy =>
                policy.RequireRole(Roles.Editor));

            options.AddPolicy(Policies.RequireAuthenticatedUser, policy =>
                policy.RequireAuthenticatedUser());

            // Composite policies (multiple roles)
            options.AddPolicy(Policies.AdminOrSuperAdmin, policy =>
                policy.RequireRole(Roles.SuperAdmin, Roles.Admin));

            options.AddPolicy(Policies.ManagerOrAbove, policy =>
                policy.RequireRole(Roles.SuperAdmin, Roles.Admin, Roles.Manager));

            options.AddPolicy(Policies.EditorOrAbove, policy =>
                policy.RequireRole(Roles.SuperAdmin, Roles.Admin, Roles.Manager, Roles.Editor));
        });
        return services;
    }
}
