using Microsoft.Extensions.Localization;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Core.Localization;

public class LocalizationInterceptor(MudLocalizer stringLocalizer) : ILocalizationInterceptor
{
    public LocalizedString Handle(string key, params object[] arguments) => stringLocalizer[key, arguments];
}
