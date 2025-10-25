using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UpdateUserProfileDto
{
    [Required]
    [MaxLength(500)]
    public string FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
