using System.Text.Json;

namespace Optivem.AtddAccelerator.EShop.Monolith.Core.Services.External;

public class ErpGateway
{
    private readonly HttpClient _httpClient;

    public ErpGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<decimal> GetUnitPrice(long productId)
    {
        var response = await _httpClient.GetAsync($"https://dummyjson.com/products/{productId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        var price = jsonDoc.RootElement.GetProperty("price").GetDecimal();

        return price;
    }
}
