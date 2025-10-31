using Microsoft.Playwright;
using System.Text.RegularExpressions;

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

        // Act
        await page.GotoAsync($"{baseUrl}/shop.html");

        var productIdInput = page.Locator("[aria-label='Product ID']");
        await productIdInput.FillAsync("10");

        var quantityInput = page.Locator("[aria-label='Quantity']");
        await quantityInput.FillAsync("5");

        var placeOrderButton = page.Locator("[aria-label='Place Order']");
        await placeOrderButton.ClickAsync();

        // Wait for confirmation message to appear
        var confirmationMessage = page.Locator("[role='alert']");
        await confirmationMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = TestConfiguration.WaitSeconds * 1000 });

        var confirmationMessageText = await confirmationMessage.TextContentAsync();

        // Assert
        var pattern = new Regex(@"Success! Order has been created with Order Number ([\w-]+) and Total Price \$(\d+(?:\.\d{2})?)");
        var match = pattern.Match(confirmationMessageText ?? "");
        
        Assert.True(match.Success, $"Confirmation message should match expected pattern. Actual: {confirmationMessageText}");

        var totalPriceString = match.Groups[2].Value;
        var totalPrice = decimal.Parse(totalPriceString);
        Assert.True(totalPrice > 0, $"Total price should be positive. Actual: {totalPrice}");

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

        await page.GotoAsync($"{baseUrl}/shop.html");

        var productIdInput = page.Locator("[aria-label='Product ID']");
        await productIdInput.FillAsync("11");

        var quantityInput = page.Locator("[aria-label='Quantity']");
        await quantityInput.FillAsync("3");

        var placeOrderButton = page.Locator("[aria-label='Place Order']");
        await placeOrderButton.ClickAsync();

        // Wait for confirmation message and extract order number
        var confirmationMessage = page.Locator("[role='alert']");
        await confirmationMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = TestConfiguration.WaitSeconds * 1000 });

        var confirmationMessageText = await confirmationMessage.TextContentAsync();
        var pattern = new Regex(@"Success! Order has been created with Order Number ([\w-]+)");
        var match = pattern.Match(confirmationMessageText ?? "");
        Assert.True(match.Success, "Should extract order number from confirmation message");
        var orderNumber = match.Groups[1].Value;

        // Act - Navigate to Order History and search for the order
        await page.GotoAsync($"{baseUrl}/");
        
        var orderHistoryLink = page.Locator("a[href='/order-history.html']");
        await orderHistoryLink.ClickAsync();

        var orderNumberInput = page.Locator("[aria-label='Order Number']");
        await orderNumberInput.FillAsync(orderNumber);

        var searchButton = page.Locator("[aria-label='Search']");
        await searchButton.ClickAsync();

        // Wait for order details to appear
        var orderDetails = page.Locator("[role='alert']");
        await orderDetails.WaitForAsync(new LocatorWaitForOptions { Timeout = TestConfiguration.WaitSeconds * 1000 });

        var orderDetailsText = await orderDetails.TextContentAsync();

        // Assert - Verify order details heading is displayed
        Assert.Contains("Order Details", orderDetailsText);

        // Verify order details in read-only textboxes
        var displayOrderNumber = page.Locator("[aria-label='Display Order Number']");
        var displayProductId = page.Locator("[aria-label='Display Product ID']");
        var displayQuantity = page.Locator("[aria-label='Display Quantity']");
        var displayUnitPrice = page.Locator("[aria-label='Display Unit Price']");
        var displayTotalPrice = page.Locator("[aria-label='Display Total Price']");

        Assert.Equal(orderNumber, await displayOrderNumber.InputValueAsync());
        Assert.Equal("11", await displayProductId.InputValueAsync());
        Assert.Equal("3", await displayQuantity.InputValueAsync());
        
        var unitPriceValue = await displayUnitPrice.InputValueAsync();
        Assert.True(unitPriceValue?.StartsWith("$"), "Should display unit price with $ symbol");
        
        var totalPriceValue = await displayTotalPrice.InputValueAsync();
        Assert.True(totalPriceValue?.StartsWith("$"), "Should display total price with $ symbol");

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

        await page.GotoAsync($"{baseUrl}/shop.html");

        var productIdInput = page.Locator("[aria-label='Product ID']");
        await productIdInput.FillAsync("12");

        var quantityInput = page.Locator("[aria-label='Quantity']");
        await quantityInput.FillAsync("2");

        var placeOrderButton = page.Locator("[aria-label='Place Order']");
        await placeOrderButton.ClickAsync();

        // Wait for confirmation message and extract order number
        var confirmationMessage = page.Locator("[role='alert']");
        await confirmationMessage.WaitForAsync(new LocatorWaitForOptions { Timeout = TestConfiguration.WaitSeconds * 1000 });

        var confirmationMessageText = await confirmationMessage.TextContentAsync();
        var pattern = new Regex(@"Success! Order has been created with Order Number ([\w-]+)");
        var match = pattern.Match(confirmationMessageText ?? "");
        Assert.True(match.Success, "Should extract order number from confirmation message");
        var orderNumber = match.Groups[1].Value;

        // Act - Navigate to Order History and search for the order
        await page.GotoAsync($"{baseUrl}/");
        
        var orderHistoryLink = page.Locator("a[href='/order-history.html']");
        await orderHistoryLink.ClickAsync();

        var orderNumberInput = page.Locator("[aria-label='Order Number']");
        await orderNumberInput.FillAsync(orderNumber);

        var searchButton = page.Locator("[aria-label='Search']");
        await searchButton.ClickAsync();

        // Wait for order details to appear
        var orderDetails = page.Locator("[role='alert']");
        await orderDetails.WaitForAsync(new LocatorWaitForOptions { Timeout = TestConfiguration.WaitSeconds * 1000 });

        // Verify initial status is PLACED
        var displayStatusBeforeCancel = page.Locator("[aria-label='Display Status']");
        Assert.Equal("Placed", await displayStatusBeforeCancel.InputValueAsync());

        // Click Cancel Order button
        page.Dialog += (_, dialog) => dialog.AcceptAsync(); // Auto-accept the alert
        var cancelButton = page.Locator("[aria-label='Cancel Order']");
        await cancelButton.ClickAsync();

        // Wait a moment for the order to be cancelled and details refreshed
        await page.WaitForTimeoutAsync(1000);

        // Assert - Verify status changed to CANCELLED
        var displayStatusAfterCancel = page.Locator("[aria-label='Display Status']");
        Assert.Equal("Cancelled", await displayStatusAfterCancel.InputValueAsync());

        // Verify Cancel button is no longer visible (since order is already cancelled)
        var cancelButtonAfter = page.Locator("[aria-label='Cancel Order']");
        Assert.Equal(0, await cancelButtonAfter.CountAsync());

        await browser.CloseAsync();
    }
}