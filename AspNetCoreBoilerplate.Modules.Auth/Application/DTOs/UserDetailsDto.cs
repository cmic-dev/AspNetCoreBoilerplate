namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public bool IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public DateTime? LastSuccessfulLoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public RoleDto Role { get; set; } = new();
}


