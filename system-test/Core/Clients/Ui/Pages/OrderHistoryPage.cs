using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

public class OrderHistoryPage : BasePage
{
    public OrderHistoryPage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task SearchOrder(string orderNumber)
    {
        await FillInput("[aria-label='Order Number']", orderNumber);
        await ClickButton("[aria-label='Search']");
    }

    public async Task WaitForOrderDetails()
    {
        await WaitForElement("[role='alert']", TestConfiguration.WaitSeconds * 1000);
    }

    public async Task<string> GetOrderDetailsText()
    {
        return await GetText("[role='alert']");
    }

    public async Task<OrderDetailsDisplay> GetOrderDetails()
    {
        return new OrderDetailsDisplay
        {
            OrderNumber = await GetInputValue("[aria-label='Display Order Number']"),
            ProductId = await GetInputValue("[aria-label='Display Product ID']"),
            Quantity = await GetInputValue("[aria-label='Display Quantity']"),
            UnitPrice = await GetInputValue("[aria-label='Display Unit Price']"),
            TotalPrice = await GetInputValue("[aria-label='Display Total Price']"),
            Status = await GetInputValue("[aria-label='Display Status']")
        };
    }

    public async Task ClickCancelOrder()
    {
        Page.Dialog += (_, dialog) => dialog.AcceptAsync();
        await ClickButton("[aria-label='Cancel Order']");
    }

    public async Task<int> GetCancelButtonCount()
    {
        return await GetElementCount("[aria-label='Cancel Order']");
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
