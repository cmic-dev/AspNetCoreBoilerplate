using AspNetCoreBoilerplate.Web.Core.Models;
using Fluxor;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Store.Layout;

public record LayoutState
{
    public bool IsNavigrationBarOpen { get; init; }
    public bool IsMobile { get; internal init; }
    public int AppBarHeight { get; init; }
    public string ActiveRoute { get; init; } = "/";

    public static LayoutState Initial { get; } = new LayoutState()
    {
        AppBarHeight = 50,
        ActiveRoute = "/",
        IsNavigrationBarOpen = false,
        IsMobile = false
    };
}

public record ToggleNavigationBarAction();
public record OpenNavigationBarAction();
public record CloseNavigrationBarAction();

public record SetActiveRouteAction(string ActiveRoute);

public class LayoutFeatureState : Feature<LayoutState>
{
    public override string GetName() => "Layout";
    protected override LayoutState GetInitialState() => LayoutState.Initial;
}


public static class LayoutReducers
{
    [ReducerMethod]
    public static LayoutState OnToggleDrawer(LayoutState state, ToggleNavigationBarAction action) =>
        state with { IsNavigrationBarOpen = !state.IsNavigrationBarOpen };

    [ReducerMethod]
    public static LayoutState OnOpenDrawer(LayoutState state, OpenNavigationBarAction action) =>
        state with { IsNavigrationBarOpen = true };

    [ReducerMethod]
    public static LayoutState OnCloseDrawer(LayoutState state, CloseNavigrationBarAction action) =>
        state with { IsNavigrationBarOpen = false };

    [ReducerMethod]
    public static LayoutState OnSetActiveRoute(LayoutState state, SetActiveRouteAction action) =>
        state with { ActiveRoute = action.ActiveRoute };
}
