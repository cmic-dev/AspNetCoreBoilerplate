using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Store.Layout;
using AspNetCoreBoilerplate.Web.Store.UserProfile;
using Fluxor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace AspNetCoreBoilerplate.Web.Pages.Settings;

public partial class UserProfile
{
    private UpdateProfileRequestDto generalProfileModel = new();
    private UpdateEmailRequestDto emailModel = new();

    [Inject]
    private IState<UserProfileState> UserProfileState { get; init; } = default!;

    [Inject]
    private IState<LayoutState> LayoutState { get; init; } = default!;

    [Inject]
    private MudLocalizer L { get; init; } = default!;

    [Inject]
    private IDispatcher Dispatcher { get; init; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (UserProfileState.Value.UserProfile != null)
            MapUpdateProfileRequestDto(UserProfileState.Value.UserProfile);

        SubscribeToAction<FetchProfileSuccessAction>((action) => MapUpdateProfileRequestDto(action.UserProfile));
    }

    private void HandleSaveProfile()
    {
        Dispatcher.Dispatch(new UpdateProfileAction(generalProfileModel));
    }

    private void HandleUpdateEmailAddress()
    {
    }

    private void MapUpdateProfileRequestDto(UserDetailsDto userDetails)
    {
        generalProfileModel = new UpdateProfileRequestDto()
        {
            FullName = userDetails.FullName,
            Gender = userDetails.Gender,
            PhoneNumber = userDetails.PhoneNumber,
            DateOfBirth = userDetails.DateOfBirth.HasValue ? userDetails.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue) : null
        };

        emailModel = new UpdateEmailRequestDto()
        {
            NewEmail = userDetails.Email ?? "",
            Password = string.Empty
        };
    }
}
