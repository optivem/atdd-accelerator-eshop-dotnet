using System;
using Microsoft.Playwright;
using Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui.Pages;

namespace Optivem.AtddAccelerator.EShop.SystemTest.Core.Clients.Ui
{
    public class UiClient : IAsyncDisposable
    {
        private readonly string _baseUrl;
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IPage? _page;

        public UiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        protected string BaseUrl => _baseUrl;

        protected async Task<IPage> GetPage()
        {
            if (_page == null)
            {
                await InitializeBrowser();
            }
            return _page!;
        }

        private async Task InitializeBrowser()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            _page = await _browser.NewPageAsync();
        }

        public async Task<HomePage> OpenHomePage()
        {
            var page = await GetPage();
            var homePage = new HomePage(page, _baseUrl);
            await homePage.NavigateToHome();
            return homePage;
        }

        public async ValueTask DisposeAsync()
        {
            if (_browser != null)
            {
                await _browser.CloseAsync();
            }
            _playwright?.Dispose();
        }
    }
}
