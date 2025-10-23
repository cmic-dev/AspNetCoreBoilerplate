using System.Net.Http.Json;

namespace AspNetCoreBoilerplate.Web.Services;

public class PublicApiService
{
    private readonly HttpClient _httpClient;

    public PublicApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> GetAsync<T>(string url, CancellationToken ctn = default)
    {
        var response = await _httpClient.GetAsync(url, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task PostAsync(string url, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsync(url, null, ctn);
        response.EnsureSuccessStatusCode();
    }

    public async Task PostAsync(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T> PostAsync<T>(string url, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsJsonAsync(url, (object?)null, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task<T> PostAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PostAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task PutAsync(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PutAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T> PutAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PutAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task DeleteAsync(string url, CancellationToken ctn = default)
    {
        var response = await _httpClient.DeleteAsync(url, ctn);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T> DeleteAsync<T>(string url, CancellationToken ctn = default)
    {
        var response = await _httpClient.DeleteAsync(url, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }

    public async Task PatchAsync(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PatchAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
    }

    public async Task<T> PatchAsync<T>(string url, object body, CancellationToken ctn = default)
    {
        var response = await _httpClient.PatchAsJsonAsync(url, body, ctn);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ctn)
            ?? throw new InvalidOperationException("Response content was null");
    }
}
