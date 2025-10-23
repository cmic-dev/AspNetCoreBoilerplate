namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class LogoutRequestDto
{
    public string RefreshToken { get; set; } = string.Empty;
}
