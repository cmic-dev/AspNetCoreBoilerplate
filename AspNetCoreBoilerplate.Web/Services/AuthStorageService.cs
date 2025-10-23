using Blazored.LocalStorage;
using AspNetCoreBoilerplate.Web.Core.Authentication;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using System.Text.Json;

namespace AspNetCoreBoilerplate.Web.Services;

public class AuthStorageService(ILocalStorageService localStorage, ILogger<AuthStorageService> logger)
{
    private readonly ILocalStorageService _localStorage = localStorage;
    private readonly ILogger<AuthStorageService> _logger = logger;

    public async Task SaveAuthDataAsync(AuthResponseDto auth)
    {
        ArgumentNullException.ThrowIfNull(auth);

        try
        {
            await _localStorage.SetItemAsync(AuthConstants.AccessTokenKey, auth.AccessToken);
            await _localStorage.SetItemAsync(AuthConstants.RefreshTokenKey, auth.RefreshToken);
            await _localStorage.SetItemAsync(AuthConstants.TokenExpiryKey, auth.TokenExpiry);

            _logger.LogInformation("Authentication data saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save authentication data to storage");
            throw new InvalidOperationException("Failed to save authentication data to storage", ex);
        }
    }

    public async Task<AuthResponseDto?> LoadAuthDataAsync()
    {
        try
        {
            var accessToken = await _localStorage.GetItemAsync<string>(AuthConstants.AccessTokenKey);
            var refreshToken = await _localStorage.GetItemAsync<string>(AuthConstants.RefreshTokenKey);
            var tokenExpiry = await _localStorage.GetItemAsync<DateTime>(AuthConstants.TokenExpiryKey);

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogDebug("Incomplete authentication data in storage");
                return null;
            }

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = tokenExpiry,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading authentication data from storage");
            return null;
        }
    }

    public async Task ClearAuthDataAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync(AuthConstants.AccessTokenKey);
            await _localStorage.RemoveItemAsync(AuthConstants.RefreshTokenKey);
            await _localStorage.RemoveItemAsync(AuthConstants.TokenExpiryKey);

            _logger.LogInformation("Authentication data cleared successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing authentication data from storage");
        }
    }
}
