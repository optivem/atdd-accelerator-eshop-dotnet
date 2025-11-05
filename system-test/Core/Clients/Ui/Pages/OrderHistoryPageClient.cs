using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

public class OrderHistoryPageClient
{
    private readonly IPage _page;
    private readonly string _baseUrl;

    public OrderHistoryPageClient(IPage page, string baseUrl)
    {
        _page = page;
        _baseUrl = baseUrl;
    }

    public async Task NavigateToOrderHistory()
    {
        await _page.GotoAsync($"{_baseUrl}/");
        var orderHistoryLink = _page.Locator("a[href='/order-history.html']");
        await orderHistoryLink.ClickAsync();
    }

    public async Task SearchOrder(string orderNumber)
    {
        var orderNumberInput = _page.Locator("[aria-label='Order Number']");
        await orderNumberInput.FillAsync(orderNumber);

        var searchButton = _page.Locator("[aria-label='Search']");
        await searchButton.ClickAsync();
    }

    public async Task WaitForOrderDetails()
    {
        var orderDetails = _page.Locator("[role='alert']");
        await orderDetails.WaitForAsync(new LocatorWaitForOptions 
        { 
            Timeout = TestConfiguration.WaitSeconds * 1000 
        });
    }

    public async Task<string> GetOrderDetailsText()
    {
        var orderDetails = _page.Locator("[role='alert']");
        return await orderDetails.TextContentAsync() ?? string.Empty;
    }

    public async Task<OrderDetailsDisplay> GetOrderDetails()
    {
        var displayOrderNumber = _page.Locator("[aria-label='Display Order Number']");
        var displayProductId = _page.Locator("[aria-label='Display Product ID']");
        var displayQuantity = _page.Locator("[aria-label='Display Quantity']");
        var displayUnitPrice = _page.Locator("[aria-label='Display Unit Price']");
        var displayTotalPrice = _page.Locator("[aria-label='Display Total Price']");
        var displayStatus = _page.Locator("[aria-label='Display Status']");

        return new OrderDetailsDisplay
        {
            OrderNumber = await displayOrderNumber.InputValueAsync() ?? string.Empty,
            ProductId = await displayProductId.InputValueAsync() ?? string.Empty,
            Quantity = await displayQuantity.InputValueAsync() ?? string.Empty,
            UnitPrice = await displayUnitPrice.InputValueAsync() ?? string.Empty,
            TotalPrice = await displayTotalPrice.InputValueAsync() ?? string.Empty,
            Status = await displayStatus.InputValueAsync() ?? string.Empty
        };
    }

    public async Task ClickCancelOrder()
    {
        _page.Dialog += (_, dialog) => dialog.AcceptAsync();
        var cancelButton = _page.Locator("[aria-label='Cancel Order']");
        await cancelButton.ClickAsync();
    }

    public async Task<int> GetCancelButtonCount()
    {
        var cancelButton = _page.Locator("[aria-label='Cancel Order']");
        return await cancelButton.CountAsync();
    }
}

public class OrderDetailsDisplay
{
    public string OrderNumber { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string UnitPrice { get; set; } = string.Empty;
    public string TotalPrice { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
