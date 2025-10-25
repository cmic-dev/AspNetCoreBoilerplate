using System.ComponentModel.DataAnnotations;

namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class LoginRequestDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
