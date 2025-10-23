namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UserInfoDto
{
    public UserInfoDto(Guid userId, string userName, string displayName, string? email, string role, string roleDisplayName)
    {
        UserId = userId;
        UserName = userName;
        DisplayName = displayName;
        Email = email;
        Role = role;
        RoleDisplayName = roleDisplayName;
    }

    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public string RoleDisplayName { get; set; } = string.Empty;
}
