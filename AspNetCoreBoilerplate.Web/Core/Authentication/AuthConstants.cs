namespace AspNetCoreBoilerplate.Web.Core.Authentication;

public class AuthConstants
{
    public const string LoginPath = "/login";

    public const string Scheme = "Bearer";
    public const string AccessTokenKey = "accessToken";
    public const string RefreshTokenKey = "refreshToken";
    public const string TokenExpiryKey = "tokenExpiry";

    public const int TokenRefreshBufferMinutes = 2;
}
