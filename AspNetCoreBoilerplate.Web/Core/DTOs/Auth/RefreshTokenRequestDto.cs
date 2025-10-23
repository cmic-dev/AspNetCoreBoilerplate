namespace AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

public class RefreshTokenRequestDto
{
    public RefreshTokenRequestDto()
    {

    }

    public RefreshTokenRequestDto(string? refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public string RefreshToken { get; set; } = string.Empty;
}
