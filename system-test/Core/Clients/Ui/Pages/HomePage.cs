using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

public class HomePage : BasePage
{
    public HomePage(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task NavigateToHome()
    {
        await NavigateToUrl("/");
    }

    public async Task<ShopPagePage> GoToShop()
    {
        await NavigateToUrl("/shop.html");
        return new ShopPagePage(Page, BaseUrl);
    }

    public async Task<OrderHistoryPage> GoToOrderHistory()
    {
        var orderHistoryLink = GetLocator("a[href='/order-history.html']");
        await orderHistoryLink.ClickAsync();
        return new OrderHistoryPage(Page, BaseUrl);
    }

    public async Task<bool> IsShopLinkVisible()
    {
        var shopLink = GetLocator("a[href='/shop.html']");
        return await shopLink.IsVisibleAsync();
    }

    public async Task<bool> IsOrderHistoryLinkVisible()
    {
        var orderHistoryLink = GetLocator("a[href='/order-history.html']");
        return await orderHistoryLink.IsVisibleAsync();
    }
}
