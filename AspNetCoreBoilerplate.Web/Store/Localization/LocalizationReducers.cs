using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.Localization;

// Reducers
public static class LocalizationReducers
{
    [ReducerMethod]
    public static LocalizationState OnLoadTranslations(LocalizationState state, LoadTranslationsAction action)
    {
        return state with { IsLoading = true };
    }

    [ReducerMethod]
    public static LocalizationState OnLoadTranslationsSuccess(LocalizationState state, LoadTranslationsSuccessAction action)
    {
        return state with
        {
            CurrentCulture = action.Culture,
            Translations = action.Translations,
            IsLoading = false
        };
    }

    [ReducerMethod]
    public static LocalizationState OnLoadTranslationsFailure(LocalizationState state, LoadTranslationsFailureAction action)
    {
        return state with { IsLoading = false };
    }
}
