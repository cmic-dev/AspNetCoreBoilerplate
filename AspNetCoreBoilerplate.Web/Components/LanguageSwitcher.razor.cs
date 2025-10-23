using AspNetCoreBoilerplate.Web.Store.Localization;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace AspNetCoreBoilerplate.Web.Components;

public partial class LanguageSwitcher
{
    [Inject]
    private IState<LocalizationState> LocalizationState { get; set; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    private void OnCultureChanged(string culture)
    {
        if (culture == LocalizationState.Value.CurrentCulture) return;

        Dispatcher.Dispatch(new SetCultureAction(culture));
    }
}
