using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Core.Models;
using Fluxor;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Store.UserProfile;

public record UserProfileState
{
    public bool IsLoading { get; init; }
    public bool IsUpdating { get; set; }

    public UserDetailsDto? UserProfile { get; init; }
    public string? ErrorMessage { get; init; }

    public static UserProfileState Initial => new()
    {
        IsLoading = false,
        UserProfile = null,
        ErrorMessage = null
    };
}

public record FetchProfileAction;
public record FetchProfileSuccessAction(UserDetailsDto UserProfile);
public record FetchProfileFailedAction(string ErrorMessage);

public record UpdateProfileAction(UpdateProfileRequestDto UserProfile);
public record UpdateProfileSuccessAction(UserDetailsDto UserProfile);
public record UpdateProfileFailedAction(string ErrorMessage);

public record UpdateEmailAddressAction(UpdateProfileRequestDto UserProfile);
public record UpdateEmailAddressSuccessAction(UserDetailsDto UserProfile);
public record UpdateEmailAddressFailedAction(string ErrorMessage);

public class UserProfileFeature : Feature<UserProfileState>, IFeatureHasSettingsNavItem
{
    public override string GetName() => "UserProfile";
    protected override UserProfileState GetInitialState() => UserProfileState.Initial;

    public IEnumerable<SettingsNavItem> GetNavItems() =>
    [
        new SettingsNavItem("Page_Settings_Profile", "/settings/profile", Icons.Material.Filled.Person)
    ];
}
