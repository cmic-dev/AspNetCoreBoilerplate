namespace AspNetCoreBoilerplate.Modules.Auth.Application.DTOs;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime TokenExpiry { get; init; }
    public string RefreshToken { get; set; } = string.Empty;

    public UserInfoDto UserInfo { get; set; } = default(UserInfoDto);
}
