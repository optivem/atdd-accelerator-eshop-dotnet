using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Dtos;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Controllers;

public class OrderControllerClient : ApiClient
{
    public OrderControllerClient(string baseUrl) : base(baseUrl)
    {
    }

    public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
    {
        return await PostAsync<PlaceOrderResponse>("/api/orders", request);
    }

    public async Task<GetOrderResponse> GetOrder(string orderNumber)
    {
        return await GetAsync<GetOrderResponse>($"/api/orders/{orderNumber}");
    }

    public async Task CancelOrder(string orderNumber)
    {
        await DeleteAsync($"/api/orders/{orderNumber}");
    }
}
