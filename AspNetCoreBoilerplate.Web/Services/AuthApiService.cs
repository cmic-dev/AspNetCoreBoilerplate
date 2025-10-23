using AspNetCoreBoilerplate.Web.Core;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using System.Net.Http.Json;

namespace AspNetCoreBoilerplate.Web.Services;

public class AuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly PrivateApiService _privateApiService;

    public AuthApiService(HttpClient httpClient, PrivateApiService privateApiService)
    {
        _httpClient = httpClient;
        _privateApiService = privateApiService;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/{AppConstants.API_VERSION}/Auth/login", request, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task LogutAsync(LogoutRequestDto request, CancellationToken ctn = default)
    {
        await _privateApiService.PostAsync($"/api/{AppConstants.API_VERSION}/Auth/logout", request, ctn);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/{AppConstants.API_VERSION}/Auth/refresh-token", request, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AuthResponseDto>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }
}
