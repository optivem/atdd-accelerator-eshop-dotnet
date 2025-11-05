using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    protected static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    protected HttpClient HttpClient => _httpClient;

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseBody, JsonOptions)!;
    }

    protected async Task<T> PostAsync<T>(string endpoint, object request)
    {
        var response = await _httpClient.PostAsJsonAsync(endpoint, request, JsonOptions);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseBody, JsonOptions)!;
    }

    protected async Task DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> IsHealthy()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/echo");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
