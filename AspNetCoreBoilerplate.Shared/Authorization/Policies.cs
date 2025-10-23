namespace AspNetCoreBoilerplate.Shared.Authorization;

public static class Policies
{
    public const string RequireSuperAdminRole = "RequireSuperAdminRole";
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequireModeratorRole = "RequireModeratorRole";
    public const string RequireManagerRole = "RequireManagerRole";
    public const string RequireEditorRole = "RequireEditorRole";
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";

    public const string AdminOrSuperAdmin = "AdminOrSuperAdmin";
    public const string ManagerOrAbove = "ManagerOrAbove";
    public const string EditorOrAbove = "EditorOrAbove";
}
