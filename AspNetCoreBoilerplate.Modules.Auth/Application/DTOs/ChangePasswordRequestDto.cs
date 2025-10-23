using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string NewPassword { get; set; } = string.Empty;
}
