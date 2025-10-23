using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.Models;
using AspNetCoreBoilerplate.Web.Store.Layout;
using AspNetCoreBoilerplate.Web.Store.UserProfile;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Components;

public partial class NavigationBar : IDisposable
{
    private HashSet<string> _expandedGroups = new();

    [Inject]
    private IState<LayoutState> LayoutState { get; init; } = default!;

    [Inject]
    private IState<UserProfileState> UserProfileState { get; init; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = default!;

    [Inject]
    private MudLocalizer L { get; init; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; init; } = default!;

    [Inject]
    private IStore Store { get; init; } = default!;

    private IEnumerable<NavBarItem> VisibleNavigationItems { get; set; } = [];

    protected override void OnInitialized()
    {
        base.OnInitialized();

        // Set initial active route based on current URL
        var currentRoute = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
        Dispatcher.Dispatch(new SetActiveRouteAction($"/{currentRoute}"));

        // Subscribe to navigation changes
        NavigationManager.LocationChanged += OnLocationChanged;

        SubscribeToAction<FetchProfileSuccessAction>((action) =>
        {
            var navigationItems = Store.Features.Values
                .OfType<IFeatureHasNavItem>()
                .SelectMany(x => x.GetNavItems());
            VisibleNavigationItems = navigationItems
                .Where(item => item.IsVisible(action.UserProfile.Role.Name));
            StateHasChanged();
        });
    }

    private bool HasVisibleChildren(NavBarItem item) =>
        item.Children?.Any(c => UserProfileState.Value.UserProfile != null
            && c.IsVisible(UserProfileState.Value.UserProfile.Role.Name)) ?? false;

    private void HandleDrawerOpenChanged(bool isOpen) =>
        Dispatcher.Dispatch(new CloseNavigrationBarAction());

    private IEnumerable<NavBarItem> GetVisibleChildren(NavBarItem item) => UserProfileState.Value.UserProfile is null ? [] :
        item.Children?.Where(c => c.IsVisible(UserProfileState.Value.UserProfile.Role.Name)) ?? Enumerable.Empty<NavBarItem>();

    private void HandleNavLinkClick(string href)
    {
        NavigationManager.NavigateTo(href);
        // Close drawer on mobile after navigation
        if (LayoutState.Value.IsMobile)
        {
            Dispatcher.Dispatch(new CloseNavigrationBarAction());
        }
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var route = NavigationManager.ToBaseRelativePath(e.Location);
        Dispatcher.Dispatch(new SetActiveRouteAction($"/{route}"));
        StateHasChanged();
    }

    private bool IsGroupExpanded(string href) =>
        _expandedGroups.Contains(href);

    private void HandleGroupExpandedChanged(string href, bool expanded)
    {
        if (expanded)
            _expandedGroups.Add(href);
        else
            _expandedGroups.Remove(href);
    }

    public void Dispose() =>
        NavigationManager.LocationChanged -= OnLocationChanged;
}
