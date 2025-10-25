namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class RevokeTokenRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
