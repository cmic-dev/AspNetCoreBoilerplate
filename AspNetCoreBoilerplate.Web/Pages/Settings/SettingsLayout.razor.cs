using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.Models;
using AspNetCoreBoilerplate.Web.Store.UserProfile;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Pages.Settings;

public partial class SettingsLayout
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    private IState<UserProfileState> UserProfileState { get; init; } = default!;

    [Inject]
    private IStore Store { get; init; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;

    [Inject]
    private MudLocalizer L { get; init; } = default!;

    private IEnumerable<SettingsNavItem> VisibleNavigationItems { get; set; } = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();

        LoadNavItem();

        SubscribeToAction<FetchProfileSuccessAction>(action =>
        {
            LoadNavItem();
        });
    }

    private void HandleNavLinkClick(string href) =>
        NavigationManager.NavigateTo(href);

    private void LoadNavItem()
    {
        if(UserProfileState.Value.UserProfile != null)
        {
            var navigationItems = Store.Features.Values
                .OfType<IFeatureHasSettingsNavItem>()
                .SelectMany(x => x.GetNavItems());
            VisibleNavigationItems = navigationItems
                .Where(item => item.IsVisible(UserProfileState.Value.UserProfile.Role.Name));
            StateHasChanged();
        }
    }

    private void ChangeSection(int section)
    {

    }
}
