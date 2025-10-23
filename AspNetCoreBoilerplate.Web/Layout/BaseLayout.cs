using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Store.Auth;
using AspNetCoreBoilerplate.Web.Store.Localization;
using AspNetCoreBoilerplate.Web.Store.Theme;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Layout;

public abstract class BaseLayout : FluxorLayout
{
    [Inject]
    private IDispatcher Dispatcher { get; init; } = default!;

    [Inject]
    protected IState<ThemeState> ThemeState { get; init; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;

    protected override void OnInitialized()
    {
        SubscribeToAction<LoadTokenFromStorageCompletedAction>(OnLoadTokenFromStorageCompleted);
        SubscribeToAction<LogoutSuccessAction>(OnLogoutSuccess);

        // Load data from local storage
        Dispatcher.Dispatch(new LoadTokenFromStorageAction());

        Dispatcher.Dispatch(new LoadThemeFromStorageAction());
        Dispatcher.Dispatch(new LoadLocalizationFromStorageAction());

        base.OnInitialized();
    }

    private void OnLoadTokenFromStorageCompleted(LoadTokenFromStorageCompletedAction action)
    {
        if (action.Auth is null)
        {
            NavigationManager.NavigateTo(AuthConstants.LoginPath);
        }
        else
        {
            var currentRoute = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
            NavigationManager.NavigateTo(currentRoute);
        }
    }

    private void OnLogoutSuccess(LogoutSuccessAction action)
    {
        NavigationManager.NavigateTo(AuthConstants.LoginPath);
    }
}
