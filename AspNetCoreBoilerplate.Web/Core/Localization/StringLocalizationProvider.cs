using AspNetCoreBoilerplate.Web.Store.Localization;
using Fluxor;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Core.Localization;

public class StringLocalizationProvider : MudLocalizer
{
    private readonly IState<LocalizationState> _localizationState;
    public StringLocalizationProvider(IState<LocalizationState> localizationState)
    {
        _localizationState = localizationState;
    }

    public override LocalizedString this[string name]
    {
        get
        {
            if (_localizationState.Value.Translations.TryGetValue(name, out var value))
            {
                return new LocalizedString(name, value, false);
            }
            return new LocalizedString(name, name.Replace("_", " "), true);
        }
    }

    public override LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            if (_localizationState.Value.Translations.TryGetValue(name, out var value))
            {
                var formatted = string.Format(value, arguments);
                return new LocalizedString(name, formatted, false);
            }
            return new LocalizedString(name, string.Format(name.Replace("_", " "), arguments), true);
        }
    }
}
