using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Controllers;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Api.Dtos;

namespace Optivem.AtddAccelerator.EShop.SystemTest.E2eTests;

public class ApiE2eTest
{
    private readonly OrderControllerClient _orderClient;

    public ApiE2eTest()
    {
        _orderClient = new OrderControllerClient(TestConfiguration.BaseUrl);
    }

    [Fact]
    public async Task PlaceOrder_ShouldReturnOrderNumber()
    {
        // Arrange
        var request = new PlaceOrderRequest
        {
            ProductId = 10,
            Quantity = 5
        };

        // Act
        var response = await _orderClient.PlaceOrder(request);

        // Assert
        Assert.NotNull(response.OrderNumber);
        Assert.True(response.OrderNumber.StartsWith("ORD-"), "Order number should start with ORD-");
        Assert.True(response.TotalPrice > 0, "Total price should be positive");
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOrderDetails()
    {
        // Arrange - First place an order
        var placeOrderRequest = new PlaceOrderRequest
        {
            ProductId = 11,
            Quantity = 3
        };

        var placeOrderResponse = await _orderClient.PlaceOrder(placeOrderRequest);
        var orderNumber = placeOrderResponse.OrderNumber;
        
        // Act - Get the order details
        var getOrderResponse = await _orderClient.GetOrder(orderNumber);

        // Assert
        Assert.Equal(orderNumber, getOrderResponse.OrderNumber);
        Assert.Equal(11L, getOrderResponse.ProductId);
        Assert.Equal(3, getOrderResponse.Quantity);
        Assert.True(getOrderResponse.UnitPrice > 0, "Unit price should be positive");
        Assert.True(getOrderResponse.TotalPrice > 0, "Total price should be positive");
        Assert.Equal("Placed", getOrderResponse.Status);
    }

    [Fact]
    public async Task CancelOrder_ShouldSetStatusToCancelled()
    {
        // Arrange - First place an order
        var placeOrderRequest = new PlaceOrderRequest
        {
            ProductId = 12,
            Quantity = 2
        };

        var placeOrderResponse = await _orderClient.PlaceOrder(placeOrderRequest);
        var orderNumber = placeOrderResponse.OrderNumber;
        
        // Act - Cancel the order
        await _orderClient.CancelOrder(orderNumber);

        // Assert - Verify order status is CANCELLED
        var getOrderResponse = await _orderClient.GetOrder(orderNumber);
        Assert.Equal("Cancelled", getOrderResponse.Status);
    }
}