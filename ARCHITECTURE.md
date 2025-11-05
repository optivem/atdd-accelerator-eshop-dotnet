# Client Architecture - Visual Guide

## Directory Structure

```
system-test/
??? Core/
?   ??? Clients/
?       ??? Api/                              # API Testing Layer
?       ?   ??? ApiClient.cs                  # Base HTTP client (abstract functionality)
?       ?   ??? Dtos/                         # Data Transfer Objects
?       ?   ?   ??? PlaceOrderRequest.cs      # Request DTO
?       ?   ?   ??? PlaceOrderResponse.cs     # Response DTO
?       ?   ?   ??? GetOrderResponse.cs       # Response DTO
?       ?   ??? Controllers/                  # Controller-specific clients
?       ?       ??? OrderControllerClient.cs  # ? maps to monolith/Controllers/OrderController.cs
?       ?       ??? EchoControllerClient.cs   # ? maps to monolith/Controllers/EchoController.cs
?       ??? Ui/                               # UI Testing Layer  
?           ??? Pages/                        # Page Object Model
?               ??? ShopPageClient.cs         # ? interacts with /shop.html
?               ??? OrderHistoryPageClient.cs # ? interacts with /order-history.html
??? E2eTests/
?   ??? ApiE2eTest.cs                         # Uses OrderControllerClient
?   ??? UiE2eTest.cs                          # Uses ShopPageClient, OrderHistoryPageClient
??? SmokeTests/
    ??? ApiSmokeTest.cs
    ??? UiSmokeTest.cs
```

## Class Hierarchy

### API Clients

```
ApiClient (base class)
??? Properties:
?   ??? HttpClient _httpClient
??? Methods:
?   ??? GetAsync<T>(string endpoint)
?   ??? PostAsync<T>(string endpoint, object request)
?   ??? DeleteAsync(string endpoint)
?   ??? IsHealthy()
?
??? OrderControllerClient
?   ??? Methods:
?       ??? PlaceOrder(PlaceOrderRequest) ? POST /api/orders
?       ??? GetOrder(string orderNumber) ? GET /api/orders/{orderNumber}
?       ??? CancelOrder(string orderNumber) ? DELETE /api/orders/{orderNumber}
?
??? EchoControllerClient
    ??? Methods:
        ??? Echo() ? GET /api/echo
```

### UI Clients (Page Objects)

```
ShopPageClient
??? Properties:
?   ??? IPage _page
?   ??? string _baseUrl
??? Methods:
?   ??? NavigateToShop()
?   ??? FillProductId(string productId)
?   ??? FillQuantity(string quantity)
?   ??? ClickPlaceOrder()
?   ??? GetConfirmationMessage()
?   ??? ParseConfirmationMessage(string) ? OrderConfirmation
?   ??? ExtractOrderNumber(string) ? string
??? Returns:
    ??? OrderConfirmation record (OrderNumber, TotalPrice)

OrderHistoryPageClient
??? Properties:
?   ??? IPage _page
?   ??? string _baseUrl
??? Methods:
?   ??? NavigateToOrderHistory()
?   ??? SearchOrder(string orderNumber)
?   ??? WaitForOrderDetails()
?   ??? GetOrderDetailsText()
?   ??? GetOrderDetails() ? OrderDetailsDisplay
?   ??? ClickCancelOrder()
?   ??? GetCancelButtonCount() ? int
??? Returns:
    ??? OrderDetailsDisplay class (OrderNumber, ProductId, Quantity, UnitPrice, TotalPrice, Status)
```

## Data Flow

### API Test Flow
```
Test (ApiE2eTest)
    ?
    creates OrderControllerClient(baseUrl)
    ?
    calls orderClient.PlaceOrder(request)
    ?
    OrderControllerClient inherits from ApiClient
    ?
    uses PostAsync<PlaceOrderResponse>("/api/orders", request)
    ?
    ApiClient.PostAsync ? HttpClient ? Monolith API
    ?
    Response ? Deserialized to PlaceOrderResponse
    ?
    Returned to Test for assertions
```

### UI Test Flow
```
Test (UiE2eTest)
    ?
    creates Playwright ? Browser ? Page
    ?
    creates ShopPageClient(page, baseUrl)
    ?
    calls shopPage.NavigateToShop()
    ?
    calls shopPage.FillProductId("10")
    ?
    calls shopPage.FillQuantity("5")
    ?
    calls shopPage.ClickPlaceOrder()
    ?
    ShopPageClient uses Playwright locators
    ?
    Interacts with real browser ? Monolith UI
    ?
    calls shopPage.GetConfirmationMessage()
    ?
    calls shopPage.ParseConfirmationMessage(message)
    ?
    Returns OrderConfirmation ? Test for assertions
```

## Mapping to Monolith

### Controller Mapping
```
Monolith                              Test Client
?????????????????????????????????????????????????????????????????????
monolith/Controllers/                 system-test/Core/Clients/Api/Controllers/
??? OrderController.cs                ??? OrderControllerClient.cs
?   ??? PlaceOrder()                  ?   ??? PlaceOrder()
?   ??? GetOrder()                    ?   ??? GetOrder()
?   ??? CancelOrder()                 ?   ??? CancelOrder()
?                                     ?
??? EchoController.cs                 ??? EchoControllerClient.cs
?   ??? Get()                         ?   ??? Echo()
?                                     ?
??? TodosController.cs                ??? (future: TodosControllerClient.cs)
    ??? GetTodo()                         ??? GetTodo()
```

### Page Mapping
```
Monolith UI                           Test Client
?????????????????????????????????????????????????????????????????????
monolith/wwwroot/                     system-test/Core/Clients/Ui/Pages/
??? shop.html                         ??? ShopPageClient.cs
?   ??? Product ID input              ?   ??? FillProductId()
?   ??? Quantity input                ?   ??? FillQuantity()
?   ??? Place Order button            ?   ??? ClickPlaceOrder()
?   ??? Confirmation alert            ?   ??? GetConfirmationMessage()
?                                     ?
??? order-history.html                ??? OrderHistoryPageClient.cs
?   ??? Order Number input            ?   ??? SearchOrder()
?   ??? Search button                 ?   ??? WaitForOrderDetails()
?   ??? Order details display         ?   ??? GetOrderDetails()
?   ??? Cancel Order button           ?   ??? ClickCancelOrder()
?                                     ?
??? index.html                        ??? (future: HomePageClient.cs)
```

## Design Patterns

### 1. **Inheritance Pattern** (API Clients)
- Base `ApiClient` provides common HTTP operations
- Controller clients inherit and add specific methods
- Reduces code duplication
- Example:
  ```csharp
  public class OrderControllerClient : ApiClient
  {
      public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
          => await PostAsync<PlaceOrderResponse>("/api/orders", request);
  }
  ```

### 2. **Page Object Model** (UI Clients)
- Each page has its own client class
- Encapsulates page structure and interactions
- Tests don't know about HTML selectors
- Example:
  ```csharp
  var shopPage = new ShopPageClient(page, baseUrl);
  await shopPage.FillProductId("10");  // Test doesn't see [aria-label='Product ID']
  ```

### 3. **Single Responsibility Principle**
- Each controller client handles ONE controller
- Each page client handles ONE page
- Clear boundaries and responsibilities

### 4. **Composition over Direct Usage**
- Tests use clients, not raw HTTP or Playwright
- Clients compose base functionality
- Easy to swap implementations

## Benefits Summary

| Aspect | Old Structure | New Structure |
|--------|--------------|---------------|
| **Organization** | Flat `Clients/` folder | Hierarchical `Core/Clients/Api/` and `.../Ui/` |
| **API Clients** | One `ApiClient` for all | Controller-specific clients |
| **Inheritance** | None | Base `ApiClient` with shared methods |
| **Mapping** | Implicit | Explicit 1:1 with monolith structure |
| **Scalability** | Hard to add new endpoints | Easy to add new controller clients |
| **Discoverability** | Search by method name | Navigate by controller/page name |
| **Maintainability** | Changes affect all tests | Changes isolated to specific client |

## Naming Conventions

### Files
- `*Client.cs` for API clients
- `*PageClient.cs` for UI clients (Page Objects)
- `*Models.cs` for DTOs
- `*Test.cs` for test classes

### Classes
- Suffix `Client` for API clients (e.g., `OrderControllerClient`)
- Suffix `PageClient` for Page Objects (e.g., `ShopPageClient`)
- Suffix `Request` for request DTOs (e.g., `PlaceOrderRequest`)
- Suffix `Response` for response DTOs (e.g., `PlaceOrderResponse`)
- Suffix `Display` for UI DTOs (e.g., `OrderDetailsDisplay`)

### Methods
- Use same names as controller methods (e.g., `PlaceOrder()`, `GetOrder()`)
- Use descriptive names for UI actions (e.g., `FillProductId()`, `ClickPlaceOrder()`)
- Async methods suffixed with `Async` (following .NET conventions)

## Future Extensions

### Adding a New Controller Client
1. Create `system-test/Core/Clients/Api/Controllers/ProductControllerClient.cs`
2. Inherit from `ApiClient`
3. Add methods matching `monolith/Controllers/ProductController.cs`
4. Use in tests via `new ProductControllerClient(baseUrl)`

### Adding a New Page Client
1. Create `system-test/Core/Clients/Ui/Pages/CheckoutPageClient.cs`
2. Accept `IPage` and `baseUrl` in constructor
3. Add methods for page interactions
4. Return structured data via custom classes/records
5. Use in tests via `new CheckoutPageClient(page, baseUrl)`

This architecture is ready for growth! ??
