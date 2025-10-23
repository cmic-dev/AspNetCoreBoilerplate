using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.Auth;

public record AuthState
{
    public bool LoggingIn { get; init; }
    public bool LoggingOut { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime TokenExpiry { get; set; }
    public string? ErrorMessage { get; init; }

    public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);

    public static AuthState Initial { get; } = new AuthState()
    {
        LoggingIn = false,
        ErrorMessage = null,
        AccessToken = string.Empty,
        RefreshToken = string.Empty,
    };

    public bool IsExpiringSoon() => DateTime.UtcNow.AddMinutes(AuthConstants.TokenRefreshBufferMinutes) >= TokenExpiry;
}

public class AuthFeatureState : Feature<AuthState>
{
    public override string GetName() => "Auth";
    protected override AuthState GetInitialState() => AuthState.Initial;
}

public record LoginAction(LoginRequestDto Model);
public record LoginSuccessAction(AuthResponseDto Auth);
public record LoginFailedAction(string ErrorMessage);

public record LogoutAction();
public record LogoutSuccessAction();

public record RefreshTokenSuccessAction(AuthResponseDto Auth);
public record RefreshTokenFailureAction(string ErrorMessage);

public record LoadTokenFromStorageAction();
public record LoadTokenFromStorageCompletedAction(AuthResponseDto? Auth);

public record ClearAuthErrorAction();
