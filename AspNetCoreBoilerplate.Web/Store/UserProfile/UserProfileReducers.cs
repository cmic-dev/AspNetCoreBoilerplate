using Fluxor;

namespace AspNetCoreBoilerplate.Web.Store.UserProfile;

public static class UserProfileReducers
{
    [ReducerMethod]
    public static UserProfileState ReduceFetchUserProfileAction(
        UserProfileState state,
        FetchProfileAction action) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static UserProfileState ReduceFetchUserProfileSuccessAction(
        UserProfileState state,
        FetchProfileSuccessAction action) =>
        state with
        {
            IsLoading = false,
            UserProfile = action.UserProfile,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static UserProfileState ReduceFetchUserProfileFailedAction(
        UserProfileState state,
        FetchProfileFailedAction action) =>
        state with
        {
            IsLoading = false,
            UserProfile = null,
            ErrorMessage = action.ErrorMessage
        };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateProfileAction(
        UserProfileState state,
        UpdateProfileAction action) =>
        state with { IsUpdating = true, ErrorMessage = null };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateProfileSuccessAction(
        UserProfileState state,
        UpdateProfileSuccessAction action) =>
        state with
        {
            IsUpdating = false,
            UserProfile = action.UserProfile,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateProfileFailedAction(
        UserProfileState state,
        UpdateProfileFailedAction action) =>
        state with
        {
            IsUpdating = false,
            ErrorMessage = action.ErrorMessage
        };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateEmailAddressAction(
        UserProfileState state,
        UpdateEmailAddressAction action) =>
        state with { IsLoading = true, ErrorMessage = null };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateEmailAddressSuccessAction(
        UserProfileState state,
        UpdateEmailAddressSuccessAction action) =>
        state with
        {
            IsLoading = false,
            UserProfile = action.UserProfile,
            ErrorMessage = null
        };

    [ReducerMethod]
    public static UserProfileState ReduceUpdateEmailAddressFailedAction(
        UserProfileState state,
        UpdateEmailAddressFailedAction action) =>
        state with
        {
            IsLoading = false,
            ErrorMessage = action.ErrorMessage
        };
}
