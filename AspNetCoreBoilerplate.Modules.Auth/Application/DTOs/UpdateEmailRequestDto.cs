namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class UpdateEmailRequestDto
{
    public string NewEmail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
