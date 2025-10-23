using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.Models;
using Fluxor;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Store.Dashboard;

public record DashboardState
{
    public bool IsLoading { get; init; }
    public string? ErrorMessage { get; init; }
    public static DashboardState Initial { get; } = new DashboardState
    {
        IsLoading = false,
        ErrorMessage = null,
    };
}

public class DashboardFeatureState : Feature<DashboardState>, IFeatureHasNavItem
{
    public override string GetName() => "Dashboard";

    protected override DashboardState GetInitialState() =>
        DashboardState.Initial;

    public IEnumerable<NavBarItem> GetNavItems() =>
    [
        new NavBarItem("Label_Dashboard", "/", Icons.Material.Filled.Home),
    ];
}
