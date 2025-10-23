using Fluxor;
using Microsoft.JSInterop;
using System.Globalization;
using System.Net.Http.Json;

namespace AspNetCoreBoilerplate.Web.Store.Localization;

public class LocalizationEffects
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public LocalizationEffects(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    [EffectMethod]
    public async Task HandleLoadLocalizationFromStorage(LoadLocalizationFromStorageAction action, IDispatcher dispatcher)
    {
        try
        {
            var culture = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "culture");
            var selectedCulture = culture ?? "en-US";

            var cultureInfo = new CultureInfo(selectedCulture);
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            dispatcher.Dispatch(new LoadTranslationsAction(selectedCulture));
        }
        catch
        {
            dispatcher.Dispatch(new LoadTranslationsAction("en-US"));
        }
    }

    [EffectMethod]
    public async Task HandleSetCulture(SetCultureAction action, IDispatcher dispatcher)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "culture", action.Culture);

        var cultureInfo = new CultureInfo(action.Culture);

        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

        dispatcher.Dispatch(new LoadTranslationsAction(action.Culture));
    }

    [EffectMethod]
    public async Task HandleLoadTranslations(LoadTranslationsAction action, IDispatcher dispatcher)
    {
        try
        {
            var filePath = $"i18n/{action.Culture}.json";
            var translations = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>(filePath);

            if (translations != null)
            {
                dispatcher.Dispatch(new LoadTranslationsSuccessAction(translations, action.Culture));
            }
            else
            {
                dispatcher.Dispatch(new LoadTranslationsFailureAction("Translations not found"));
            }
        }
        catch (Exception ex)
        {
            if (action.Culture != "en-US")
            {
                try
                {
                    var translations = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>("i18n/en-US.json");
                    dispatcher.Dispatch(new LoadTranslationsSuccessAction(translations ?? new(), "en-US"));
                }
                catch
                {
                    dispatcher.Dispatch(new LoadTranslationsFailureAction(ex.Message));
                }
            }
            else
            {
                dispatcher.Dispatch(new LoadTranslationsFailureAction(ex.Message));
            }
        }
    }
}
