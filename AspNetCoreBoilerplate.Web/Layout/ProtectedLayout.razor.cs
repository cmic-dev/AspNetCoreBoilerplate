using Fluxor;
using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Layout;

public partial class ProtectedLayout
{
    [Inject]
    private IDispatcher Dispatcher { get; init; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }
}
