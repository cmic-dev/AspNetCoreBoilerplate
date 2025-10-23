using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Core.Extensions;
using AspNetCoreBoilerplate.Web.Services;
using AspNetCoreBoilerplate.Web.Store.UserProfile;
using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.Auth;

public class AuthEffects(AuthStorageService authStorageService, AuthApiService authApiService, CustomAuthenticationStateProvider authenticationStateProvider)
{
    [EffectMethod]
    public async Task HandleLoadTokenFromStorageCompletedAction(LoadTokenFromStorageCompletedAction action, IDispatcher dispatcher)
    {
        if (action.Auth != null)
        {
            dispatcher.Dispatch(new FetchProfileAction());
            await authStorageService.SaveAuthDataAsync(action.Auth);
        }
    }

    [EffectMethod]
    public async Task HandleLoginAction(LoginAction action, IDispatcher dispatcher)
    {
        try
        {
            var authResponse = await authApiService.LoginAsync(action.Model);
            dispatcher.Dispatch(new LoginSuccessAction(authResponse));
        }
        catch (HttpRequestException ex)
        {
            dispatcher.Dispatch(new LoginFailedAction(ex.ToHumanReadableLoginErrorMessage()));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new LoginFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public async Task HandleLogout(LogoutAction action, IDispatcher dispatcher)
    {
        try
        {
            var storedAuth = await authStorageService.LoadAuthDataAsync();
            await authApiService.LogutAsync(new LogoutRequestDto
            {
                RefreshToken = storedAuth!.RefreshToken
            });
        }
        finally
        {
            dispatcher.Dispatch(new LogoutSuccessAction());
        }
    }

    [EffectMethod]
    public async Task HandleLogoutSuccess(LogoutSuccessAction action, IDispatcher dispatcher)
    {
        authenticationStateProvider.MarkUserAsLoggedOut();
        await authStorageService.ClearAuthDataAsync();
    }

    [EffectMethod]
    public async Task HandleLoginSuccessAction(LoginSuccessAction action, IDispatcher dispatcher)
    {
        if (action.Auth != null)
        {
            await authStorageService.SaveAuthDataAsync(action.Auth);
            dispatcher.Dispatch(new FetchProfileAction());
        }
    }

    [EffectMethod]
    public async Task HandleRefreshTokenSuccessAction(RefreshTokenSuccessAction action, IDispatcher dispatcher)
    {
        if (action.Auth != null)
        {
            await authStorageService.SaveAuthDataAsync(action.Auth);
            dispatcher.Dispatch(new FetchProfileAction());
        }
    }

    [EffectMethod]
    public async Task HandleRefreshTokenFailedAction(RefreshTokenFailureAction action, IDispatcher Dispatcher)
    {
        await authStorageService.ClearAuthDataAsync();
        authenticationStateProvider.MarkUserAsLoggedOut();
    }

    [EffectMethod]
    public async Task HandleLoadUserFromStorageAction(LoadTokenFromStorageAction action, IDispatcher dispatcher)
    {
        var storedAuth = await authStorageService.LoadAuthDataAsync();
        if (storedAuth is null)
        {
            dispatcher.Dispatch(new LoadTokenFromStorageCompletedAction(null));
            return;
        }

        if (!storedAuth.IsExpiringSoon())
        {
            dispatcher.Dispatch(new LoadTokenFromStorageCompletedAction(storedAuth));
            return;
        }

        try
        {
            var refreshTokenResponse = await authApiService.RefreshTokenAsync(
                new RefreshTokenRequestDto(storedAuth.RefreshToken));
            dispatcher.Dispatch(new LoadTokenFromStorageCompletedAction(refreshTokenResponse));
        }
        catch
        {
            dispatcher.Dispatch(new RefreshTokenFailureAction("Session expired."));
            dispatcher.Dispatch(new LoadTokenFromStorageCompletedAction(null));
        }
    }
}
