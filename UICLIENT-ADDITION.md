# ? Added: UiClient Base Class

## What Was Missing

Similar to how `ApiClient` serves as the base class for all API controller clients, we needed a `UiClient` base class for all UI page clients.

## The Problem

### Before
```
Core/Clients/Ui/
??? Pages/
    ??? ShopPageClient.cs              ? Duplicate Playwright code
    ??? OrderHistoryPageClient.cs      ? Duplicate Playwright code
```

Both page clients had duplicate code for common Playwright operations like:
- Navigating to URLs
- Filling input fields
- Clicking buttons
- Getting text content
- Waiting for elements

## The Solution

### After
```
Core/Clients/Ui/
??? UiClient.cs                        ? Base class with common Playwright methods
??? Pages/
    ??? ShopPageClient.cs              ? Inherits from UiClient
    ??? OrderHistoryPageClient.cs      ? Inherits from UiClient
```

## UiClient Base Class

### Properties
```csharp
protected readonly IPage Page;
protected readonly string BaseUrl;
```

### Protected Methods
```csharp
NavigateToUrl(string relativeUrl)      // Navigate to a URL
GetLocator(string selector)            // Get a Playwright locator
FillInput(string selector, string value)   // Fill an input field
ClickButton(string selector)           // Click a button
GetText(string selector)               // Get text content
GetInputValue(string selector)         // Get input value
WaitForElement(string selector, int timeoutMs)  // Wait for element
GetElementCount(string selector)       // Get count of elements
```

## Benefits

### 1. **Symmetry with ApiClient**
```
ApiClient (base for API clients)    UiClient (base for UI clients)
     ?                                        ?
OrderControllerClient               ShopPageClient
EchoControllerClient                OrderHistoryPageClient
```

### 2. **Code Reuse**
**Before:**
```csharp
public class ShopPageClient
{
    private readonly IPage _page;
    private readonly string _baseUrl;
    
    public async Task FillProductId(string productId)
    {
        var productIdInput = _page.Locator("[aria-label='Product ID']");
        await productIdInput.FillAsync(productId);
    }
}
```

**After:**
```csharp
public class ShopPageClient : UiClient
{
    public async Task FillProductId(string productId)
    {
        await FillInput("[aria-label='Product ID']", productId);
    }
}
```

### 3. **Consistency**
- All Playwright interactions go through base class methods
- Easier to modify behavior globally
- Consistent patterns across all page clients

### 4. **Maintainability**
- If Playwright API changes, update only `UiClient`
- Common operations defined once
- Less duplication = fewer bugs

### 5. **Testability**
- Can mock `UiClient` for unit testing page clients
- Clear abstraction layer
- Easier to test page-specific logic in isolation

## Example Usage

### ShopPageClient
```csharp
public class ShopPageClient : UiClient
{
    public ShopPageClient(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task NavigateToShop()
    {
        await NavigateToUrl("/shop.html");  // ? Uses base class method
    }

    public async Task FillProductId(string productId)
    {
        await FillInput("[aria-label='Product ID']", productId);  // ? Uses base class method
    }

    public async Task ClickPlaceOrder()
    {
        await ClickButton("[aria-label='Place Order']");  // ? Uses base class method
    }
}
```

### OrderHistoryPageClient
```csharp
public class OrderHistoryPageClient : UiClient
{
    public OrderHistoryPageClient(IPage page, string baseUrl) : base(page, baseUrl)
    {
    }

    public async Task SearchOrder(string orderNumber)
    {
        await FillInput("[aria-label='Order Number']", orderNumber);  // ? Uses base class method
        await ClickButton("[aria-label='Search']");  // ? Uses base class method
    }

    public async Task<OrderDetailsDisplay> GetOrderDetails()
    {
        return new OrderDetailsDisplay
        {
            OrderNumber = await GetInputValue("[aria-label='Display Order Number']"),  // ? Uses base class method
            ProductId = await GetInputValue("[aria-label='Display Product ID']"),      // ? Uses base class method
            // ... more fields
        };
    }
}
```

## Comparison with ApiClient Pattern

### API Layer
```
ApiClient (base)
??? Protected: GetAsync<T>(), PostAsync<T>(), DeleteAsync()
??? HttpClient _httpClient
??? Inheritors:
    ??? OrderControllerClient ? PlaceOrder(), GetOrder(), CancelOrder()
    ??? EchoControllerClient ? Echo()
```

### UI Layer
```
UiClient (base)
??? Protected: NavigateToUrl(), FillInput(), ClickButton(), GetText()
??? IPage Page, string BaseUrl
??? Inheritors:
    ??? ShopPageClient ? NavigateToShop(), FillProductId(), ClickPlaceOrder()
    ??? OrderHistoryPageClient ? NavigateToOrderHistory(), SearchOrder(), ClickCancelOrder()
```

**Perfect symmetry!** ??

## Alignment with Java

The Java version has a similar pattern:
```java
// Java
public abstract class UiClient {
    protected final Page page;
    protected final String baseUrl;
    
    protected void navigateToUrl(String relativeUrl) { ... }
    protected void fillInput(String selector, String value) { ... }
    // ... more methods
}

public class ShopPageClient extends UiClient { ... }
public class OrderHistoryPageClient extends UiClient { ... }
```

Our .NET implementation now matches this pattern! ?

## Files Changed

### Created
- ? `system-test/Core/Clients/Ui/UiClient.cs` - Base UI client class

### Updated
- ? `system-test/Core/Clients/Ui/Pages/ShopPageClient.cs` - Now inherits from UiClient
- ? `system-test/Core/Clients/Ui/Pages/OrderHistoryPageClient.cs` - Now inherits from UiClient

### Documentation Updated
- ? `STEP-01-SUMMARY.md` - Added UiClient section
- ? `ARCHITECTURE.md` - Updated class hierarchy
- ? `ARCHITECTURE-COMPLETE.md` - Updated structure table

## Build Status

```
? Build successful
? All tests passing
? Zero compilation errors
? Perfect symmetry with ApiClient pattern
```

## Summary

The UI client architecture now mirrors the API client architecture:

| Layer | Base Class | Inheritors | Pattern |
|-------|-----------|------------|---------|
| **API** | `ApiClient` | `OrderControllerClient`, `EchoControllerClient` | ? Inheritance |
| **UI** | `UiClient` | `ShopPageClient`, `OrderHistoryPageClient` | ? Inheritance |

Both layers now follow the same clean inheritance pattern! ??

This completes the symmetrical client architecture matching the Java reference implementation.
