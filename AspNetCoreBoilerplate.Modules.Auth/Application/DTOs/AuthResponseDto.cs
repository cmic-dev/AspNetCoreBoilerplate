namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }
}
