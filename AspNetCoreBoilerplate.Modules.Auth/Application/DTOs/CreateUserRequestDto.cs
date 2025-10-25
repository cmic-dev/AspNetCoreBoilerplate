using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class CreateUserRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
    [RegularExpression(
            @"^[a-zA-Z][a-zA-Z0-9_-]*$",
            ErrorMessage = "Username must start with a letter and contain only alphanumeric characters, underscores, and hyphens")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }

    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Password { get; set; }

    [Required]
    public Guid RoleId { get; set; }
}
