namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class RevokeTokenRequestDto
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
