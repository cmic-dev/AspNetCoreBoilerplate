using AspNetCoreBoilerplate.Web.Store.Layout;
using AspNetCoreBoilerplate.Web.Store.Theme;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Layout;

public partial class PageLayout
{
    private string CardStyle => $"margin-top: {LayoutState.Value.AppBarHeight}px; height: calc(100vh - {LayoutState.Value.AppBarHeight}px);";
    private string CardContentClass => Scrollable ? "overflow-y-scroll" : "overflow-y-hidden";

    [Parameter]
    public bool Scrollable { get; set; } = false;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Inject]
    private IState<LayoutState> LayoutState { get; set; } = default!;
}
