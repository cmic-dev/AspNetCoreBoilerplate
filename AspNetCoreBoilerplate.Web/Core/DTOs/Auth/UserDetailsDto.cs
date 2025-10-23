namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public bool IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public DateTime? LastSuccessfulLoginAt { get; set; }

    public string GetLastSuccessfulLoginAt()
    {
        if (LastSuccessfulLoginAt.HasValue)
            return LastSuccessfulLoginAt.Value.ToLocalTime().ToString("MMM dd, yyyy 'at' hh:mm tt");

        return "--:--";
    }

    public DateTime? PasswordChangedAt { get; set; }
    public RoleDto Role { get; set; } = new();
}