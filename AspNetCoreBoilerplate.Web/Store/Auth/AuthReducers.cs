using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.Auth;

public static class AuthReducers
{
    [ReducerMethod]
    public static AuthState OnLogin(AuthState state, LoginAction action)
        => state with { LoggingIn = true };

    [ReducerMethod]
    public static AuthState OnLoginSuccess(AuthState state, LoginSuccessAction action)
    {
        return state with
        {
            AccessToken = action.Auth.AccessToken,
            RefreshToken = action.Auth.RefreshToken,
            TokenExpiry = action.Auth.TokenExpiry,
            LoggingIn = false
        };
    }

    [ReducerMethod]
    public static AuthState OnLoginFailed(AuthState state, LoginFailedAction action)
        => state with { ErrorMessage = action.ErrorMessage, LoggingIn = false };

    [ReducerMethod]
    public static AuthState OnLoadTokenFromStorageCompleted(AuthState state, LoadTokenFromStorageCompletedAction action)
    {
        if (action.Auth is null)
            return AuthState.Initial;

        return state with
        {
            AccessToken = action.Auth.AccessToken,
            RefreshToken = action.Auth.RefreshToken,
            TokenExpiry = action.Auth.TokenExpiry,
            LoggingIn = false
        };
    }

    [ReducerMethod]
    public static AuthState OnLogout(AuthState state, LogoutAction action) =>
        state with { LoggingOut = true };

    [ReducerMethod]
    public static AuthState OnLogoutSuccess(AuthState state, LogoutSuccessAction action) =>
        AuthState.Initial;

    [ReducerMethod]
    public static AuthState OnRefreshTokenSuccess(AuthState state, RefreshTokenSuccessAction action)
    {
        return state with
        {
            AccessToken = action.Auth.AccessToken,
            RefreshToken = action.Auth.RefreshToken,
            TokenExpiry = action.Auth.TokenExpiry,
            LoggingIn = false
        };
    }
}
