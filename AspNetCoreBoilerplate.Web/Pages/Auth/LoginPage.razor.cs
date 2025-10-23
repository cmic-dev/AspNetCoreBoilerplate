using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Store.Auth;
using AspNetCoreBoilerplate.Web.Store.Localization;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Pages.Auth;

public partial class LoginPage
{
    private LoginRequestDto loginModel = new();

    [Inject]
    private MudLocalizer L { get; init; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = default!;

    [Inject]
    private IState<AuthState> AuthState { get; init; } = default!;

    [Inject]
    private NavigationManager Navigation { get; init; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SubscribeToAction<LoadTranslationsSuccessAction>(action => StateHasChanged());
        Dispatcher.Dispatch(new ClearAuthErrorAction());
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (AuthState.Value.IsAuthenticated)
        {
            Navigation.NavigateTo("/");
        }
    }

    private void HandleLogin()
    {
        Dispatcher.Dispatch(new LoginAction(loginModel));
    }
}
