using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.Localization;

public record LocalizationState
{
    public string CurrentCulture { get; init; } = "en-US";
    public Dictionary<string, string> Translations { get; init; } = new();
    public bool IsLoading { get; init; } = false;

    public string this[string name]
    {
        get
        {
            if (Translations.ContainsKey(name))
                return Translations[name];
            return name;
        }
    }

    public string this[string name, params object[] arguments]
    {
        get
        {
            if (Translations.ContainsKey(name))
                return Translations[name];
            return name;
        }
    }

    public static LocalizationState Init => new LocalizationState
    {
        CurrentCulture = "en-US",
        Translations = new Dictionary<string, string>(),
        IsLoading = false
    };
}

public record LoadLocalizationFromStorageAction();
public record SetCultureAction(string Culture);
public record LoadTranslationsAction(string Culture);
public record LoadTranslationsSuccessAction(Dictionary<string, string> Translations, string Culture);
public record LoadTranslationsFailureAction(string Error);

public class LocalizationFeature : Feature<LocalizationState>
{
    public override string GetName() => "Localization";
    protected override LocalizationState GetInitialState() => LocalizationState.Init;
}
