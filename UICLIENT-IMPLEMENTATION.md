# ? UiClient Implementation - Complete

## Overview

Implemented `UiClient` as a high-level client that manages the Playwright browser lifecycle and provides entry point to the UI test automation through the home page.

## Architecture Pattern

### Java-Style UiClient Pattern

Following the Java reference implementation, `UiClient`:
1. **Manages browser lifecycle** (Playwright, Browser, Page)
2. **Provides entry point** via `OpenHomePage()`
3. **Implements IAsyncDisposable** for proper cleanup
4. **Navigation flows through page hierarchy**

## Structure

```
system-test/Core/Clients/Ui/
??? UiClient.cs                       ? Manages browser, opens HomePage
??? Pages/
?   ??? BasePage.cs                   ? Base class for all pages
?   ??? HomePage.cs                   ? Entry point, navigates to other pages
?   ??? ShopPagePage.cs               ? Shop functionality
?   ??? OrderHistoryPage.cs           ? Order history functionality
```

## UiClient Implementation

### Responsibilities

```csharp
public class UiClient : IAsyncDisposable
{
    // 1. Manages Playwright browser lifecycle
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    
    // 2. Provides entry point
    public async Task<HomePage> OpenHomePage()
    
    // 3. Ensures proper cleanup
    public async ValueTask DisposeAsync()
}
```

### Key Features

1. **Lazy Initialization** - Browser created only when needed
2. **Single Responsibility** - Manages ONLY browser lifecycle
3. **Entry Point Pattern** - All tests start with `OpenHomePage()`
4. **Resource Management** - Proper disposal via `IAsyncDisposable`

## Page Hierarchy

### Navigation Flow

```
UiClient
    ?
    OpenHomePage()
    ?
HomePage
    ??? GoToShop() ? ShopPagePage
    ??? GoToOrderHistory() ? OrderHistoryPage
```

### HomePage as Navigation Hub

```csharp
public class HomePage : BasePage
{
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
}
```

**Key Design Decision:** HomePage returns new page instances, allowing fluent navigation.

## Usage Pattern

### Before (Direct Playwright)

```csharp
using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
var page = await browser.NewPageAsync();

await page.GotoAsync($"{baseUrl}/shop.html");
var productIdInput = page.Locator("[aria-label='Product ID']");
await productIdInput.FillAsync("10");
// ... more Playwright code

await browser.CloseAsync();
```

### After (UiClient Pattern)

```csharp
await using var uiClient = new UiClient(TestConfiguration.BaseUrl);
var homePage = await uiClient.OpenHomePage();
var shopPage = await homePage.GoToShop();

await shopPage.FillProductId("10");
await shopPage.FillQuantity("5");
await shopPage.ClickPlaceOrder();

// Browser automatically disposed when uiClient goes out of scope
```

## Complete Example

### Test Using UiClient

```csharp
[Fact]
public async Task ShouldCalculateTotalOrderPrice()
{
    // Arrange - Create UiClient and navigate to shop
    await using var uiClient = new UiClient(TestConfiguration.BaseUrl);
    var homePage = await uiClient.OpenHomePage();
    var shopPage = await homePage.GoToShop();

    // Act - Perform actions on shop page
    await shopPage.FillProductId("10");
    await shopPage.FillQuantity("5");
    await shopPage.ClickPlaceOrder();
    var confirmationMessageText = await shopPage.GetConfirmationMessage();

    // Assert
    var confirmation = shopPage.ParseConfirmationMessage(confirmationMessageText);
    Assert.True(confirmation.TotalPrice > 0);
    
    // Browser automatically cleaned up via DisposeAsync
}
```

## Benefits

### 1. **Centralized Browser Management**
- All browser lifecycle code in one place
- Consistent browser configuration across tests
- Easy to modify browser options globally

### 2. **Clear Entry Point**
- All tests start with `OpenHomePage()`
- Mirrors real user journey
- Consistent pattern across all UI tests

### 3. **Proper Resource Management**
```csharp
await using var uiClient = new UiClient(baseUrl);
// Browser automatically disposed at end of scope
```

### 4. **Page Object Hierarchy**
- HomePage is the navigation hub
- Other pages don't know about navigation
- Single Responsibility Principle

### 5. **Testability**
- UiClient can be mocked for unit tests
- Pages don't depend on browser creation
- Clear boundaries between concerns

## Comparison: ApiClient vs UiClient

### ApiClient Pattern
```
ApiClient
??? Manages: HttpClient
??? Entry Point: Controller-specific clients
??? Usage: Direct instantiation of controller clients

Test:
var orderClient = new OrderControllerClient(baseUrl);
var response = await orderClient.PlaceOrder(request);
```

### UiClient Pattern
```
UiClient
??? Manages: Playwright + Browser + Page
??? Entry Point: OpenHomePage()
??? Usage: Navigate through page hierarchy

Test:
await using var uiClient = new UiClient(baseUrl);
var homePage = await uiClient.OpenHomePage();
var shopPage = await homePage.GoToShop();
```

**Key Difference:** UiClient manages complex lifecycle, ApiClient is stateless.

## File Organization

### Before
```
system-test/Core/Clients/Ui/
??? Pages/
    ??? ShopPageClient.cs             ? Had NavigateToShop()
    ??? OrderHistoryPageClient.cs     ? Had NavigateToOrderHistory()
```

Problems:
- Each page responsible for its own navigation
- No clear entry point
- Tests managed browser lifecycle directly

### After
```
system-test/Core/Clients/Ui/
??? UiClient.cs                       ? Browser lifecycle + entry point
??? Pages/
    ??? BasePage.cs                   ? Common Playwright operations
    ??? HomePage.cs                   ? Navigation hub
    ??? ShopPagePage.cs               ? Pure shop functionality
    ??? OrderHistoryPage.cs           ? Pure order history functionality
```

Benefits:
- Clear separation of concerns
- HomePage is navigation hub
- Pages focus on their domain functionality
- UiClient manages browser lifecycle

## Design Decisions

### 1. Why HomePage as Entry Point?

**Rationale:** Mirrors real user behavior
- Users start at home page
- Users navigate from home to other pages
- Makes tests more realistic

**Alternative:** Could have allowed direct navigation to any page
- But would break page object encapsulation
- HomePage acts as navigation "hub"

### 2. Why IAsyncDisposable?

**Rationale:** Proper resource cleanup
```csharp
await using var uiClient = new UiClient(baseUrl);
// Automatically calls DisposeAsync at end of scope
```

**Alternative:** Manual cleanup
```csharp
var uiClient = new UiClient(baseUrl);
try {
    // test code
} finally {
    await uiClient.DisposeAsync();
}
```
But using pattern is cleaner!

### 3. Why Return New Page Instances?

**Rationale:** Fluent API
```csharp
var shopPage = await homePage.GoToShop();
var orderHistoryPage = await homePage.GoToOrderHistory();
```

**Alternative:** Void methods that navigate internally
```csharp
await homePage.GoToShop();
// Now homePage is actually on shop page? Confusing!
```

### 4. Why Remove Navigation from Individual Pages?

**Rationale:** Single Responsibility
- `ShopPagePage` shouldn't know HOW to get to shop
- `OrderHistoryPage` shouldn't know HOW to get to order history
- HomePage knows the navigation structure
- Pages focus on their domain functionality

## Alignment with Java

The implementation follows the Java pattern:

### Java
```java
public class UiClient implements AutoCloseable {
    private Playwright playwright;
    private Browser browser;
    private Page page;
    
    public HomePage openHomePage() { ... }
    
    @Override
    public void close() { ... }
}
```

### C#
```csharp
public class UiClient : IAsyncDisposable
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IPage? _page;
    
    public async Task<HomePage> OpenHomePage() { ... }
    
    public async ValueTask DisposeAsync() { ... }
}
```

? **Perfect alignment!**

## Testing

All tests now use the new pattern:

```bash
dotnet test --filter "FullyQualifiedName~UiE2eTest"
```

? Build successful  
? All tests passing  
? Proper resource cleanup verified

## Summary

The `UiClient` implementation provides:

1. ? **Browser lifecycle management** - Centralized, consistent
2. ? **Clear entry point** - Always starts with `OpenHomePage()`
3. ? **Page hierarchy** - HomePage navigates to other pages
4. ? **Proper cleanup** - Via `IAsyncDisposable`
5. ? **Separation of concerns** - Pages focus on functionality, not navigation
6. ? **Java alignment** - Matches reference implementation pattern

This completes the UiClient implementation! ??
