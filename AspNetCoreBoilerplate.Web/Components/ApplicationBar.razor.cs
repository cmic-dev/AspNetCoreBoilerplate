using AspNetCoreBoilerplate.Web.Store.Layout;
using AspNetCoreBoilerplate.Web.Store.Theme;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Components;

public partial class ApplicationBar
{
    [Inject]
    private IState<LayoutState> LayoutState { get; set; } = default!;

    [Inject]
    private IState<ThemeState> ThemeState { get; set; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    private void ToggleDrawer()
    {
        Dispatcher.Dispatch(new ToggleNavigationBarAction());
    }

    private void ToggleTheme()
    {
        Dispatcher.Dispatch(new ToggleThemeAction());
        Dispatcher.Dispatch(new SaveThemeToStorageAction());
    }

    private string GetThemeIcon() => ThemeState.Value.IsDarkMode ?
        Icons.Material.Filled.LightMode : Icons.Material.Filled.DarkMode;
}
