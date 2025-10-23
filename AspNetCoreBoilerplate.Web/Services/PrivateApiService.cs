using Fluxor;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using AspNetCoreBoilerplate.Web.Core.Models;
using AspNetCoreBoilerplate.Web.Core.DTOs.Auth;
using AspNetCoreBoilerplate.Web.Store.Auth;

namespace AspNetCoreBoilerplate.Web.Services;

public class PrivateApiService(HttpClient httpClient, IState<AuthState> authState, IDispatcher dispatcher)
{
    private static readonly SemaphoreSlim _refreshTokenSemaphore = new(1, 1);
    private static readonly int _maxRetryAttempts = 3;

    public async Task<T> GetAsync<T>(string url, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.GetAsync(url, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    public async Task PostAsync(string url, CancellationToken ctn = default)
    {
        await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PostAsync(url, null, innerCtn);
            response.EnsureSuccessStatusCode();
            return true;
        }, ctn);
    }

    public async Task PostAsync(string url, object body, CancellationToken ctn = default)
    {
        await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PostAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return true;
        }, ctn);
    }

    public async Task<T> PostAsync<T>(string url, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PostAsJsonAsync(url, (object?)null, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    public async Task<T> PostAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PostAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    public async Task PutAsync(string url, object body, CancellationToken ctn = default)
    {
        await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PutAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return true;
        }, ctn);
    }

    public async Task<T> PutAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PutAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    public async Task DeleteAsync(string url, CancellationToken ctn = default)
    {
        await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.DeleteAsync(url, innerCtn);
            response.EnsureSuccessStatusCode();
            return true;
        }, ctn);
    }

    public async Task<T> DeleteAsync<T>(string url, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.DeleteAsync(url, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    public async Task PatchAsync(string url, object body, CancellationToken ctn = default)
    {
        await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PatchAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return true;
        }, ctn);
    }

    public async Task<T> PatchAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        return await ExecuteWithAuthorizationAsync(async (httpClient, innerCtn) =>
        {
            var response = await httpClient.PatchAsJsonAsync(url, body, innerCtn);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(innerCtn)
                ?? throw new InvalidOperationException("Response content was null");
        }, ctn);
    }

    private async Task<T> ExecuteWithAuthorizationAsync<T>(Func<HttpClient, CancellationToken, Task<T>> operation, CancellationToken ctn = default)
    {
        var authStateValue = authState.Value;

        if (!authStateValue.IsAuthenticated)
            throw new UnauthorizedAccessException("User is not authenticated");

        if (authStateValue.IsExpiringSoon())
            return await ExecuteWithRefreshTokenAsync(operation, ctn);

        try
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authStateValue.AccessToken);
            return await operation(httpClient, ctn);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error during authorized operation", ex);
        }
    }

    private async Task<T> ExecuteWithRefreshTokenAsync<T>(
        Func<HttpClient, CancellationToken, Task<T>> operation,
        CancellationToken ctn = default)
    {
        await _refreshTokenSemaphore.WaitAsync(ctn);
        try
        {
            var retryCount = 0;

            while (retryCount < _maxRetryAttempts)
            {
                try
                {
                    var response = await httpClient.PostAsJsonAsync(
                        "api/Auth/refresh-token",
                        new RefreshTokenRequestDto(authState.Value.RefreshToken),
                        ctn);

                    response.EnsureSuccessStatusCode();

                    var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>(ctn)
                        ?? throw new InvalidOperationException("Failed to deserialize refresh token response");

                    dispatcher.Dispatch(new RefreshTokenSuccessAction(authResponse));

                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

                    return await operation(httpClient, ctn);
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;
                    if (retryCount >= _maxRetryAttempts)
                    {
                        dispatcher.Dispatch(new RefreshTokenFailureAction($"HTTP error: {ex.Message}"));
                        throw;
                    }
                    await Task.Delay(500, ctn);
                }
                catch (Exception ex)
                {
                    dispatcher.Dispatch(new RefreshTokenFailureAction($"Unexpected error: {ex.Message}"));
                    throw;
                }
            }

            dispatcher.Dispatch(new RefreshTokenFailureAction("Failed to refresh token after multiple attempts"));
            throw new UnauthorizedAccessException("Failed to refresh token");
        }
        finally
        {
            _refreshTokenSemaphore.Release();
        }
    }
}
