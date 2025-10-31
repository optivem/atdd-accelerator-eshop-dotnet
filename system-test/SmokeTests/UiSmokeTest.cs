using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.EShop.SystemTest.SmokeTests;

public class UiSmokeTest
{
    [Fact]
    public async Task Home_ShouldReturnHtmlContent()
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var page = await browser.NewPageAsync();
        
        // Navigate and get response
        var response = await page.GotoAsync(TestConfiguration.BaseUrl);
        
        // Assert
        Assert.NotNull(response);
        Assert.Equal(200, response.Status);
        
        // Check content type is HTML
        var headers = await response.AllHeadersAsync();
        var contentType = headers.TryGetValue("content-type", out var ct) ? ct : "";
        Assert.True(!string.IsNullOrEmpty(contentType) && contentType.Contains("text/html"), 
                   $"Content-Type should be text/html, but was: {contentType}");
        
        // Check HTML structure using Playwright's content method
        var pageContent = await page.ContentAsync();
        Assert.Contains("<html", pageContent);
        Assert.Contains("</html>", pageContent);
        
        await browser.CloseAsync();
    }
}