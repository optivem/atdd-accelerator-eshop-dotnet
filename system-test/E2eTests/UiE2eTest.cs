using Microsoft.Playwright;
using Optivem.AtddAccelerator.EShop.SystemTest.Clients;

namespace Optivem.AtddAccelerator.EShop.SystemTest.E2eTests;

public class UiE2eTest
{
    [Fact]
    public async Task ShouldCalculateTotalOrderPrice()
    {
        // Arrange
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        var baseUrl = TestConfiguration.BaseUrl;

        var shopPage = new ShopPageClient(page, baseUrl);

        // Act
        await shopPage.NavigateToShop();
        await shopPage.FillProductId("10");
        await shopPage.FillQuantity("5");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();

        // Assert
        var confirmation = shopPage.ParseConfirmationMessage(confirmationMessageText);
        Assert.True(confirmation.TotalPrice > 0, $"Total price should be positive. Actual: {confirmation.TotalPrice}");

        await browser.CloseAsync();
    }

    [Fact]
    public async Task ShouldRetrieveOrderHistory()
    {
        // Arrange - First place an order to get an order number
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        var baseUrl = TestConfiguration.BaseUrl;

        var shopPage = new ShopPageClient(page, baseUrl);
        var orderHistoryPage = new OrderHistoryPageClient(page, baseUrl);

        await shopPage.NavigateToShop();
        await shopPage.FillProductId("11");
        await shopPage.FillQuantity("3");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();
        var orderNumber = shopPage.ExtractOrderNumber(confirmationMessageText);

        // Act - Navigate to Order History and search for the order
        await orderHistoryPage.NavigateToOrderHistory();
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

        await browser.CloseAsync();
    }

    [Fact]
    public async Task ShouldCancelOrder()
    {
        // Arrange - First place an order
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var page = await browser.NewPageAsync();
        var baseUrl = TestConfiguration.BaseUrl;

        var shopPage = new ShopPageClient(page, baseUrl);
        var orderHistoryPage = new OrderHistoryPageClient(page, baseUrl);

        await shopPage.NavigateToShop();
        await shopPage.FillProductId("12");
        await shopPage.FillQuantity("2");
        await shopPage.ClickPlaceOrder();

        var confirmationMessageText = await shopPage.GetConfirmationMessage();
        var orderNumber = shopPage.ExtractOrderNumber(confirmationMessageText);

        // Act - Navigate to Order History and search for the order
        await orderHistoryPage.NavigateToOrderHistory();
        await orderHistoryPage.SearchOrder(orderNumber);
        await orderHistoryPage.WaitForOrderDetails();

        // Verify initial status is PLACED
        var orderDetailsBeforeCancel = await orderHistoryPage.GetOrderDetails();
        Assert.Equal("Placed", orderDetailsBeforeCancel.Status);

        // Click Cancel Order button
        await orderHistoryPage.ClickCancelOrder();

        // Wait a moment for the order to be cancelled and details refreshed
        await page.WaitForTimeoutAsync(1000);

        // Assert - Verify status changed to CANCELLED
        var orderDetailsAfterCancel = await orderHistoryPage.GetOrderDetails();
        Assert.Equal("Cancelled", orderDetailsAfterCancel.Status);

        // Verify Cancel button is no longer visible (since order is already cancelled)
        var cancelButtonCount = await orderHistoryPage.GetCancelButtonCount();
        Assert.Equal(0, cancelButtonCount);

        await browser.CloseAsync();
    }
}