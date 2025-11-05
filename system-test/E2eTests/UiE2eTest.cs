using Microsoft.Playwright;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

namespace Optivem.AtddAccelerator.EShop.SystemTest.E2eTests;

public class UiE2eTest
{
    [Fact]
    public async Task ShouldCalculateTotalOrderPrice()
    {
        // Arrange
        await using var uiClient = new UiClient(TestConfiguration.BaseUrl);
        var homePage = await uiClient.OpenHomePage();
        var shopPage = await homePage.GoToShop();

        // Act
        await shopPage.FillProductId("10");
        await shopPage.FillQuantity("5");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();

        // Assert
        var confirmation = shopPage.ParseConfirmationMessage(confirmationMessageText);
        Assert.True(confirmation.TotalPrice > 0, $"Total price should be positive. Actual: {confirmation.TotalPrice}");
    }

    [Fact]
    public async Task ShouldRetrieveOrderHistory()
    {
        // Arrange - First place an order to get an order number
        await using var uiClient = new UiClient(TestConfiguration.BaseUrl);
        var homePage = await uiClient.OpenHomePage();
        var shopPage = await homePage.GoToShop();

        await shopPage.FillProductId("11");
        await shopPage.FillQuantity("3");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();
        var orderNumber = shopPage.ExtractOrderNumber(confirmationMessageText);

        // Act - Navigate to Order History and search for the order
        var homePage2 = await uiClient.OpenHomePage();
        var orderHistoryPage = await homePage2.GoToOrderHistory();
        await orderHistoryPage.SearchOrder(orderNumber);
        await orderHistoryPage.WaitForOrderDetails();

        var orderDetailsText = await orderHistoryPage.GetOrderDetailsText();

        // Assert - Verify order details heading is displayed
        Assert.Contains("Order Details", orderDetailsText);

        // Verify order details in read-only textboxes
        var orderDetails = await orderHistoryPage.GetOrderDetails();
        Assert.Equal(orderNumber, orderDetails.OrderNumber);
        Assert.Equal("11", orderDetails.ProductId);
        Assert.Equal("3", orderDetails.Quantity);
        Assert.True(orderDetails.UnitPrice.StartsWith("$"), "Should display unit price with $ symbol");
        Assert.True(orderDetails.TotalPrice.StartsWith("$"), "Should display total price with $ symbol");
    }

    [Fact]
    public async Task ShouldCancelOrder()
    {
        // Arrange - First place an order
        await using var uiClient = new UiClient(TestConfiguration.BaseUrl);
        var homePage = await uiClient.OpenHomePage();
        var shopPage = await homePage.GoToShop();

        await shopPage.FillProductId("12");
        await shopPage.FillQuantity("2");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();
        var orderNumber = shopPage.ExtractOrderNumber(confirmationMessageText);

        // Act - Navigate to Order History and search for the order
        var homePage2 = await uiClient.OpenHomePage();
        var orderHistoryPage = await homePage2.GoToOrderHistory();
        await orderHistoryPage.SearchOrder(orderNumber);
        await orderHistoryPage.WaitForOrderDetails();

        // Verify initial status is PLACED
        var orderDetailsBeforeCancel = await orderHistoryPage.GetOrderDetails();
        Assert.Equal("Placed", orderDetailsBeforeCancel.Status);

        // Click Cancel Order button
        await orderHistoryPage.ClickCancelOrder();

        // Wait a moment for the order to be cancelled and details refreshed
        await Task.Delay(1000);

        // Assert - Verify status changed to CANCELLED
        var orderDetailsAfterCancel = await orderHistoryPage.GetOrderDetails();
        Assert.Equal("Cancelled", orderDetailsAfterCancel.Status);

        // Verify Cancel button is no longer visible (since order is already cancelled)
        var cancelButtonCount = await orderHistoryPage.GetCancelButtonCount();
        Assert.Equal(0, cancelButtonCount);
    }
}