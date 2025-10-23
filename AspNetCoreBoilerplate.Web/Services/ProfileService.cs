using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;

namespace AspNetCoreBoilerplate.Web.Services;

public class ProfileService
{
    private readonly PrivateApiService _privateApiService;
    public ProfileService(PrivateApiService privateApiService)
    {
        _privateApiService = privateApiService;
    }

    public async Task<UserDetailsDto> GetMeAsync(CancellationToken ctn = default)
    {
        return await _privateApiService.GetAsync<UserDetailsDto>($"/api/{AppConstants.API_VERSION}/Profile/me", ctn);
    }

    public async Task<UserDetailsDto> UpdateProfileAsync(UpdateProfileRequestDto request, CancellationToken ctn = default)
    {
        return await _privateApiService.PostAsync<UserDetailsDto>($"/api/{AppConstants.API_VERSION}/Profile/update-profile", request, ctn);
    }

    public async Task<UserDetailsDto> UpdateEmailAddressAsync(UpdateEmailRequestDto request, CancellationToken ctn = default)
    {
        return await _privateApiService.PostAsync<UserDetailsDto>($"/api/{AppConstants.API_VERSION}/Profile/update-email", request, ctn);
    }

    public async Task<UserDetailsDto> UpdateProfilePicture(CancellationToken ctn = default)
    {
        return await _privateApiService.PostAsync<UserDetailsDto>($"/api/{AppConstants.API_VERSION}/Profile/update-email", ctn);
    }
}
