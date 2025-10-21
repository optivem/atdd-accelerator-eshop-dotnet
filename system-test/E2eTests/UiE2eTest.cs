using Microsoft.Playwright;

namespace Optivem.AtddAccelerator.Template.SystemTest.E2eTests
{
    public class UiE2eTest
    {
        [Fact]
        public async Task FetchTodo_ShouldDisplayTodoDataInUI()
        {
            // DISCLAIMER: This is an example of a badly written test
            // which unfortunately simulates real-life software test projects.
            // This is the starting point for our ATDD Accelerator exercises.

            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync();
            var page = await browser.NewPageAsync();
            
            // Navigate to the todo page
            await page.GotoAsync("http://localhost:8080/todos");
            
            // 1. Check there's a textbox with id
            var todoIdInput = page.Locator("#todoId");
            await Assertions.Expect(todoIdInput).ToBeVisibleAsync();
            
            // 2. Input value 4 into that textbox
            await todoIdInput.FillAsync("4");
            
            // 3. Click "Fetch Todo" button (this will submit the form)
            var fetchButton = page.Locator("#fetchTodo");
            await fetchButton.ClickAsync();
            
            // 4. Wait for the page to reload and result to appear
            await page.WaitForURLAsync("**/todos");
            
            // Wait for the result div to appear
            var todoResult = page.Locator("#todoResult");
            await todoResult.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            
            var resultText = await todoResult.TextContentAsync();
            
            // Debug: Print the actual result text
            Console.WriteLine($"Actual result text: {resultText}");
            
            // Verify the todo data is displayed (more flexible checking)
            Assert.True((resultText.Contains("userId") || resultText.Contains("User ID")) && 
                       (resultText.Contains("1") || resultText.Contains(": 1")), 
                       $"Result should contain userId: 1. Actual text: {resultText}");
            Assert.True((resultText.Contains("\"id\"") || resultText.Contains("ID:")) && 
                       (resultText.Contains("4") || resultText.Contains(": 4")), 
                       $"Result should contain id: 4. Actual text: {resultText}");
            Assert.True(resultText.Contains("title", StringComparison.OrdinalIgnoreCase) || 
                       resultText.Contains("Title"), 
                       $"Result should contain title field. Actual text: {resultText}");
            Assert.True(resultText.Contains("completed", StringComparison.OrdinalIgnoreCase) || 
                       resultText.Contains("Completed"), 
                       $"Result should contain completed field. Actual text: {resultText}");
            
            await browser.CloseAsync();
        }
    }
}