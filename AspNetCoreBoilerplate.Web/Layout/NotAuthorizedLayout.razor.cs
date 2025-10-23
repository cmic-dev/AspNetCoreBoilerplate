using AspNetCoreBoilerplate.Web.Store.Theme;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Layout;

public partial class NotAuthorizedLayout
{
    [Inject]
    protected IState<ThemeState> ThemeState { get; init; } = default!;
}
