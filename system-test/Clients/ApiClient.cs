using System.Net.Http.Json;
using System.Text.Json;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Clients;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(string baseUrl)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };
    }

    public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/orders", request);
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<PlaceOrderResponse>(responseBody, JsonOptions)!;
    }

    public async Task<GetOrderResponse> GetOrder(string orderNumber)
    {
        var response = await _httpClient.GetAsync($"/api/orders/{orderNumber}");
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<GetOrderResponse>(responseBody, JsonOptions)!;
    }

    public async Task CancelOrder(string orderNumber)
    {
        var response = await _httpClient.DeleteAsync($"/api/orders/{orderNumber}");
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
