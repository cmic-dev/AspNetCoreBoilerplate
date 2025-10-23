using Blazored.LocalStorage;
using Fluxor;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Store.Theme;

public record ThemeState
{
    public bool IsDarkMode { get; init; }
    public MudTheme CurrentTheme { get; init; }
    public string PrimaryColor { get; init; } = "#F18C20";
    public string SecondaryColor { get; init; } = "#607D8B";
    public int BorderRadius { get; init; } = 4;
    public int Elevation { get; init; } = 1;

    public bool IsLoading { get; init; }

    public static ThemeState Initial => new()
    {
        IsDarkMode = false,
        CurrentTheme = CreateDefaultTheme(),
        PrimaryColor = "#F18C20",
        SecondaryColor = "#607D8B",
        BorderRadius = 4,
        Elevation = 1,
        IsLoading = false
    };

    private static MudTheme CreateDefaultTheme() => new()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#F18C20",
            Secondary = "#607D8B",
            AppbarBackground = "#F18C20",
            Background = "#F5F5F5",
            Surface = "#FFFFFF",
            DrawerBackground = "#FFFFFF",
            DrawerText = "rgba(0,0,0, 0.87)",
            Success = "#4CAF50"
        },
        PaletteDark = new PaletteDark()
        {
            Primary = "#F18C20",
            Secondary = "#607D8B",
            AppbarBackground = "#1E1E2E",
            Background = "#0F0F23",
            Surface = "#1E1E2E",
            DrawerBackground = "#1E1E2E",
            DrawerText = "rgba(255,255,255, 0.87)",
            Success = "#66BB6A"
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "4px"
        }
    };
}

public record ToggleThemeAction();
public record SetThemeAction(bool IsDarkMode);
public record SetPrimaryColorAction(string Color);
public record SetSecondaryColorAction(string Color);
public record SetBorderRadiusAction(int BorderRadius);
public record SetElevationAction(int Elevation);
public record SetFontFamilyAction(string FontFamily);
public record SetCompactModeAction(bool CompactMode);
public record LoadThemeFromStorageAction();
public record SaveThemeToStorageAction();
public record ResetThemeToDefaultAction();

public class ThemeFeature : Feature<ThemeState>
{
    public override string GetName() => "Theme";
    protected override ThemeState GetInitialState() => ThemeState.Initial;
}

public static class ThemeReducers
{
    [ReducerMethod]
    public static ThemeState OnToggleTheme(ThemeState state, ToggleThemeAction action) =>
        state with { IsDarkMode = !state.IsDarkMode };

    [ReducerMethod]
    public static ThemeState OnSetTheme(ThemeState state, SetThemeAction action) =>
        state with { IsDarkMode = action.IsDarkMode };

    [ReducerMethod]
    public static ThemeState OnSetPrimaryColor(ThemeState state, SetPrimaryColorAction action) =>
        state with
        {
            PrimaryColor = action.Color,
            CurrentTheme = UpdateTheme(state, action.Color, state.SecondaryColor, state.BorderRadius)
        };

    [ReducerMethod]
    public static ThemeState OnSetSecondaryColor(ThemeState state, SetSecondaryColorAction action) =>
        state with
        {
            SecondaryColor = action.Color,
            CurrentTheme = UpdateTheme(state, state.PrimaryColor, action.Color, state.BorderRadius)
        };

    [ReducerMethod]
    public static ThemeState OnSetBorderRadius(ThemeState state, SetBorderRadiusAction action) =>
        state with
        {
            BorderRadius = action.BorderRadius,
            CurrentTheme = UpdateTheme(state, state.PrimaryColor, state.SecondaryColor, action.BorderRadius)
        };

    [ReducerMethod]
    public static ThemeState OnSetElevation(ThemeState state, SetElevationAction action) =>
        state with { Elevation = action.Elevation };

    [ReducerMethod]
    public static ThemeState OnSetFontFamily(ThemeState state, SetFontFamilyAction action) =>
        state with
        {
            CurrentTheme = UpdateTheme(state, state.PrimaryColor, state.SecondaryColor, state.BorderRadius)
        };

    [ReducerMethod]
    public static ThemeState OnSetCompactMode(ThemeState state, SetCompactModeAction action) =>
        state with
        {
            CurrentTheme = UpdateTheme(state, state.PrimaryColor, state.SecondaryColor, state.BorderRadius)
        };

    [ReducerMethod]
    public static ThemeState OnResetThemeToDefault(ThemeState state, ResetThemeToDefaultAction action) =>
        ThemeState.Initial;

    // Safe theme update method
    private static MudTheme UpdateTheme(ThemeState state, string primary, string secondary, int borderRadius)
    {
        var baseTheme = state.CurrentTheme;

        var newTheme = new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = primary,
                Secondary = secondary,
                AppbarBackground = primary,
                Background = baseTheme.PaletteLight.Background,
                Surface = baseTheme.PaletteLight.Surface,
                DrawerBackground = baseTheme.PaletteLight.DrawerBackground,
                DrawerText = baseTheme.PaletteLight.DrawerText,
                Success = baseTheme.PaletteLight.Success
            },
            PaletteDark = new PaletteDark
            {
                Primary = primary,
                Secondary = secondary,
                AppbarBackground = "#1E1E2E",
                Background = baseTheme.PaletteDark.Background,
                Surface = baseTheme.PaletteDark.Surface,
                DrawerBackground = baseTheme.PaletteDark.DrawerBackground,
                DrawerText = baseTheme.PaletteDark.DrawerText,
                Success = baseTheme.PaletteDark.Success
            },
            LayoutProperties = new LayoutProperties()
            {
                DefaultBorderRadius = $"{borderRadius}px",
            },
        };

        return newTheme;
    }
}

public class ThemeEffects
{
    private readonly ILocalStorageService _localStorage;
    private readonly IState<ThemeState> _state;

    public ThemeEffects(ILocalStorageService localStorage, IState<ThemeState> state)
    {
        _localStorage = localStorage;
        _state = state;
    }

    [EffectMethod]
    public async Task HandleLoadThemeFromStorage(LoadThemeFromStorageAction action, IDispatcher dispatcher)
    {
        try
        {
            var isDarkModeResult = await _localStorage.GetItemAsync<bool?>("isDarkMode");
            if (isDarkModeResult.HasValue)
            {
                dispatcher.Dispatch(new SetThemeAction(isDarkModeResult.Value));
            }

            var primaryColorResult = await _localStorage.GetItemAsync<string>("primaryColor");
            if (!string.IsNullOrEmpty(primaryColorResult))
            {
                dispatcher.Dispatch(new SetPrimaryColorAction(primaryColorResult));
            }

            var secondaryColorResult = await _localStorage.GetItemAsync<string>("secondaryColor");
            if (!string.IsNullOrEmpty(secondaryColorResult))
            {
                dispatcher.Dispatch(new SetSecondaryColorAction(secondaryColorResult));
            }

            var borderRadiusResult = await _localStorage.GetItemAsync<int?>("borderRadius");
            if (borderRadiusResult.HasValue)
            {
                dispatcher.Dispatch(new SetBorderRadiusAction(borderRadiusResult.Value));
            }

            var elevationResult = await _localStorage.GetItemAsync<int?>("elevation");
            if (elevationResult.HasValue)
            {
                dispatcher.Dispatch(new SetElevationAction(elevationResult.Value));
            }

            var fontFamilyResult = await _localStorage.GetItemAsync<string>("fontFamily");
            if (!string.IsNullOrEmpty(fontFamilyResult))
            {
                dispatcher.Dispatch(new SetFontFamilyAction(fontFamilyResult));
            }

            var compactModeResult = await _localStorage.GetItemAsync<bool?>("compactMode");
            if (compactModeResult.HasValue)
            {
                dispatcher.Dispatch(new SetCompactModeAction(compactModeResult.Value));
            }
        }
        catch (Exception)
        {
        }
    }

    [EffectMethod]
    public async Task HandleSaveThemeToStorage(SaveThemeToStorageAction action, IDispatcher dispatcher)
    {
        try
        {
            await _localStorage.SetItemAsync("isDarkMode", _state.Value.IsDarkMode);
            await _localStorage.SetItemAsync("primaryColor", _state.Value.PrimaryColor);
            await _localStorage.SetItemAsync("secondaryColor", _state.Value.SecondaryColor);
            await _localStorage.SetItemAsync("borderRadius", _state.Value.BorderRadius);
            await _localStorage.SetItemAsync("elevation", _state.Value.Elevation);
        }
        catch (Exception)
        {
        }
    }
}
