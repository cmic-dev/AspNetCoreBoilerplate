namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class UserDetailsDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? Gender { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public bool IsActive { get; set; }
    public DateTime? LastSuccessfulLoginAt { get; set; }
    public DateTime? PasswordChangedAt { get; set; }
    public RoleDto Role { get; set; } = new();

    public string GetLastSuccessfulLoginAt()
    {
        if (LastSuccessfulLoginAt.HasValue)
            return LastSuccessfulLoginAt.Value.ToLongDateString();

        return "--";
    }
}