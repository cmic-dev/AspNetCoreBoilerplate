using AspNetCoreBoilerplate.Web.Core.Authentication;
using System.Text.Json;

namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    public bool IsExpiringSoon() => DateTime.UtcNow.AddMinutes(AuthConstants.TokenRefreshBufferMinutes) >= TokenExpiry;

    public bool IsExpired() => DateTime.UtcNow >= TokenExpiry;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
}
