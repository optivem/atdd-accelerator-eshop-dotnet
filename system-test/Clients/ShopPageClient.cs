using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Clients;

public class ShopPageClient
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public ShopPageClient(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateToShop()
    {
        await _page.GotoAsync($"{_baseUrl}/shop.html");
    }

    public async Task FillProductId(string productId)
    {
        var productIdInput = _page.Locator("[aria-label='Product ID']");
        await productIdInput.FillAsync(productId);
    }

    public async Task FillQuantity(string quantity)
    {
        var quantityInput = _page.Locator("[aria-label='Quantity']");
        await quantityInput.FillAsync(quantity);
    }

    public async Task ClickPlaceOrder()
    {
        var placeOrderButton = _page.Locator("[aria-label='Place Order']");
        await placeOrderButton.ClickAsync();
    }

    public async Task<string> GetConfirmationMessage()
    {
        var confirmationMessage = _page.Locator("[role='alert']");
        await confirmationMessage.WaitForAsync(new LocatorWaitForOptions 
        { 
            Timeout = TestConfiguration.WaitSeconds * 1000 
        });
        return await confirmationMessage.TextContentAsync() ?? string.Empty;
    }

    public OrderConfirmation ParseConfirmationMessage(string message)
    {
        var pattern = new Regex(@"Success! Order has been created with Order Number ([\w-]+) and Total Price \$(\d+(?:\.\d{2})?)");
        var match = pattern.Match(message);
        
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not parse confirmation message: {message}");
        }

        var orderNumber = match.Groups[1].Value;
        var totalPrice = decimal.Parse(match.Groups[2].Value);
        
        return new OrderConfirmation(orderNumber, totalPrice);
    }

    public string ExtractOrderNumber(string message)
    {
        var pattern = new Regex(@"Success! Order has been created with Order Number ([\w-]+)");
        var match = pattern.Match(message);
        
        if (!match.Success)
        {
            throw new InvalidOperationException($"Could not extract order number from message: {message}");
        }

        return match.Groups[1].Value;
    }
}

public record OrderConfirmation(string OrderNumber, decimal TotalPrice);
