using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

public abstract class BasePage
{
    protected readonly IPage Page;
    protected readonly string BaseUrl;

    protected BasePage(IPage page, string baseUrl)
    {
        Page = page;
        BaseUrl = baseUrl;
    }

    protected async Task NavigateToUrl(string relativeUrl)
    {
        await Page.GotoAsync($"{BaseUrl}{relativeUrl}");
    }

    protected ILocator GetLocator(string selector)
    {
        return Page.Locator(selector);
    }

    protected async Task FillInput(string selector, string value)
    {
        var input = Page.Locator(selector);
        await input.FillAsync(value);
    }

    protected async Task ClickButton(string selector)
    {
        var button = Page.Locator(selector);
        await button.ClickAsync();
    }

    protected async Task<string> GetText(string selector)
    {
        var element = Page.Locator(selector);
        return await element.TextContentAsync() ?? string.Empty;
    }

    protected async Task<string> GetInputValue(string selector)
    {
        var input = Page.Locator(selector);
        return await input.InputValueAsync() ?? string.Empty;
    }

    protected async Task WaitForElement(string selector, int timeoutMs)
    {
        var element = Page.Locator(selector);
        await element.WaitForAsync(new LocatorWaitForOptions { Timeout = timeoutMs });
    }

    protected async Task<int> GetElementCount(string selector)
    {
        var elements = Page.Locator(selector);
        return await elements.CountAsync();
    }
}
