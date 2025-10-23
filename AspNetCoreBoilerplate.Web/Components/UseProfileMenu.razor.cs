using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Store.Auth;
using AspNetCoreBoilerplate.Web.Store.UserProfile;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Components;

public partial class UseProfileMenu
{
    private MudMenu menuRef = default!;

    private UserDetailsDto? User => UserProfileState.Value.UserProfile;

    private string imageUrl { get; set; } = "/";

    [Inject]
    private IState<UserProfileState> UserProfileState { get; set; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    [Inject]
    private MudLocalizer L { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private async Task ResetPassword()
    {
        await menuRef.CloseMenuAsync();
        Navigation.NavigateTo("/settings?tab=account-and-security#reset-password");
    }

    private void NavigateToProfile() => Navigation.NavigateTo("/profile");

    private void NavigateToSettings() => Navigation.NavigateTo("/settings");

    private async Task Login()
    {
        await menuRef.CloseMenuAsync();
        Navigation.NavigateTo(AuthConstants.LoginPath);
    }

    private async Task Logout()
    {
        await menuRef.CloseMenuAsync();
        Dispatcher.Dispatch(new LogoutAction());
    }
}
