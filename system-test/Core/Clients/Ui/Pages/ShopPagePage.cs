using Microsoft.Playwright;
using System.Text.RegularExpressions;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

public class ShopPagePage : BasePage
{
    public ShopPagePage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task FillProductId(string productId)
    {
        await FillInput("[aria-label='Product ID']", productId);
    }

    public async Task FillQuantity(string quantity)
    {
        await FillInput("[aria-label='Quantity']", quantity);
    }

    public async Task ClickPlaceOrder()
    {
        await ClickButton("[aria-label='Place Order']");
    }

    public async Task<string> GetConfirmationMessage()
    {
        await WaitForElement("[role='alert']", TestConfiguration.WaitSeconds * 1000);
        return await GetText("[role='alert']");
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
