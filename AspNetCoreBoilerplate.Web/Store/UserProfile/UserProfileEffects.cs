using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Services;
using Fluxor;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Store.UserProfile;

public class UserProfileEffects(
    ProfileService profileService,
    CustomAuthenticationStateProvider authenticationStateProvider,
    ISnackbar snackbar)
{
    [EffectMethod]
    public async Task HandleFetchUserProfileAction(
        FetchProfileAction action,
        IDispatcher dispatcher)
    {
        try
        {
            var userProfile = await profileService.GetMeAsync();
            dispatcher.Dispatch(new FetchProfileSuccessAction(userProfile));
        }
        catch (HttpRequestException ex)
        {

        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new FetchProfileFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public Task HandleFetchUserProfileSuccessAction(
        FetchProfileSuccessAction action,
        IDispatcher dispatcher)
    {
        authenticationStateProvider.NotifyAuthChanged(action.UserProfile);
        return Task.CompletedTask;
    }

    [EffectMethod]
    public async Task HandleUpdateProfileAction(UpdateProfileAction action, IDispatcher dispatcher)
    {
        try
        {
            var updatedProfile = await profileService.UpdateProfileAsync(action.UserProfile);
            dispatcher.Dispatch(new UpdateProfileSuccessAction(updatedProfile));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new UpdateProfileFailedAction(ex.Message));
        }
    }

    [EffectMethod]
    public Task HandleUpdateProfileSuccessAction(UpdateProfileSuccessAction action, IDispatcher dispatcher)
    {
        authenticationStateProvider.NotifyAuthChanged(action.UserProfile);
        snackbar.Add("Updated");
        return Task.CompletedTask;
    }
}
