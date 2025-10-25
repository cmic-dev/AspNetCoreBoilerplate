namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class ChangePasswordRequestDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
