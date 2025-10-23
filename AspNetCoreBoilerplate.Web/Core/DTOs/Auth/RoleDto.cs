namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

