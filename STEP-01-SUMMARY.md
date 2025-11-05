# Step 01: E2E Tests - Clients Refactoring

## Overview

This step introduces the **Client Layer** pattern to the E2E tests, extracting HTTP and UI interaction logic into dedicated client classes. This follows the **Page Object Model** for UI tests and the **API Client** pattern for API tests.

The architecture is organized into a hierarchical structure that separates API clients from UI clients and further organizes them by controller and page responsibilities.

## Architecture Structure

```
system-test/Core/Clients/
??? Api/
?   ??? ApiClient.cs                      # Base HTTP client with common methods
?   ??? Dtos/                             # Data Transfer Objects
?   ?   ??? PlaceOrderRequest.cs          # DTO for placing orders
?   ?   ??? PlaceOrderResponse.cs         # DTO for order placement response
?   ?   ??? GetOrderResponse.cs           # DTO for retrieving order details
?   ??? Controllers/
?       ??? OrderControllerClient.cs      # Client for Order API endpoints
?       ??? EchoControllerClient.cs       # Client for Echo API endpoints
??? Ui/
    ??? Pages/
        ??? ShopPageClient.cs             # Page Object for Shop page
        ??? OrderHistoryPageClient.cs     # Page Object for Order History page
```

## Changes Made

### 1. Created API Client Layer

#### Base API Client (`system-test/Core/Clients/Api/ApiClient.cs`)
- Provides base HTTP functionality for all API clients
- Contains protected helper methods: `GetAsync<T>()`, `PostAsync<T>()`, `DeleteAsync()`
- Handles JSON serialization/deserialization with proper configuration
- Configured with `JsonStringEnumConverter` for enum handling
- Other API clients inherit from this base class

#### API DTOs (`system-test/Core/Clients/Api/Dtos/`)
- Centralized Data Transfer Objects in dedicated folder
- Each DTO is in its own file for better maintainability
- Files:
  - `PlaceOrderRequest.cs` - Request DTO for placing orders
  - `PlaceOrderResponse.cs` - Response DTO from placing orders
  - `GetOrderResponse.cs` - Response DTO for retrieving order details

#### Order Controller Client (`system-test/Core/Clients/Api/Controllers/OrderControllerClient.cs`)
- Inherits from `ApiClient`
- Provides methods: `PlaceOrder()`, `GetOrder()`, `CancelOrder()`
- Maps directly to `OrderController` endpoints in the monolith
- Uses DTOs from `Core.Clients.Api.Dtos` namespace

#### Echo Controller Client (`system-test/Core/Clients/Api/Controllers/EchoControllerClient.cs`)
- Inherits from `ApiClient`
- Provides method: `Echo()`
- Maps to `EchoController` endpoints

### 2. Created UI Client Layer (Page Objects)

#### Shop Page Client (`system-test/Core/Clients/Ui/Pages/ShopPageClient.cs`)
- Implements Page Object Model for the Shop page
- Methods for navigation and interactions: `NavigateToShop()`, `FillProductId()`, `FillQuantity()`, `ClickPlaceOrder()`
- Helper methods for parsing: `ParseConfirmationMessage()`, `ExtractOrderNumber()`
- Returns structured data via `OrderConfirmation` record

#### Order History Page Client (`system-test/Core/Clients/Ui/Pages/OrderHistoryPageClient.cs`)
- Implements Page Object Model for the Order History page
- Methods: `NavigateToOrderHistory()`, `SearchOrder()`, `GetOrderDetails()`, `ClickCancelOrder()`
- Returns structured data via `OrderDetailsDisplay` class

### 3. Refactored Tests

#### API E2E Tests (`system-test/E2eTests/ApiE2eTest.cs`)
- **Before**: Used generic `ApiClient` for all API calls
- **After**: Uses specific `OrderControllerClient`
- **Benefits**: 
  - Clear separation of controller responsibilities
  - Tests are more focused and easier to read
  - Follows Single Responsibility Principle

#### UI E2E Tests (`system-test/E2eTests/UiE2eTest.cs`)
- **Before**: Used `ShopPageClient` and `OrderHistoryPageClient` from `Clients` namespace
- **After**: Uses page clients from `Core.Clients.Ui.Pages` namespace
- **Benefits**:
  - Better organization following standard patterns
  - Clear separation between API and UI clients

### 4. Removed Old Structure
- Deleted `system-test/Clients/` folder
- All client code now lives under `system-test/Core/Clients/`

## Benefits of This Architecture

### 1. **Clear Separation of Concerns**
- **API clients** are separate from **UI clients**
- Each **controller** has its own client class
- Each **page** has its own page object class

### 2. **Inheritance Hierarchy**
- Base `ApiClient` provides common HTTP functionality
- Controller clients inherit and add specific endpoint methods
- Reduces duplication and promotes reuse

### 3. **Maintainability**
- If API endpoints change, only the relevant controller client needs updates
- If UI changes, only the relevant page client needs updates
- Tests remain stable and focused on behavior

### 4. **Scalability**
- Easy to add new controller clients (e.g., `ProductControllerClient`)
- Easy to add new page clients (e.g., `CheckoutPageClient`)
- Structure scales naturally as application grows

### 5. **Alignment with Monolith Structure**
- Controller clients mirror the monolith's `Controllers/`
- Makes it easy for developers to find corresponding test clients
- Reduces cognitive load when navigating codebase

## Example: Controller Client Pattern

### Monolith Controller
```csharp
// monolith/Controllers/OrderController.cs
[ApiController]
[Route("api")]
public class OrderController : ControllerBase
{
    [HttpPost("orders")]
    public async Task<ActionResult<PlaceOrderResponse>> PlaceOrder([FromBody] PlaceOrderRequest request)
    
    [HttpGet("orders/{orderNumber}")]
    public ActionResult<GetOrderResponse> GetOrder(string orderNumber)
    
    [HttpDelete("orders/{orderNumber}")]
    public IActionResult CancelOrder(string orderNumber)
}
```

### Test Controller Client
```csharp
// system-test/Core/Clients/Api/Controllers/OrderControllerClient.cs
public class OrderControllerClient : ApiClient
{
    public async Task<PlaceOrderResponse> PlaceOrder(PlaceOrderRequest request)
    public async Task<GetOrderResponse> GetOrder(string orderNumber)
    public async Task CancelOrder(string orderNumber)
}
```

**Perfect 1:1 mapping!**

## Example Comparison

### Before (Flat Structure)
```csharp
// system-test/Clients/ApiClient.cs - generic client for all endpoints
var apiClient = new ApiClient(baseUrl);
var response = await apiClient.PlaceOrder(request);
```

### After (Hierarchical Structure)
```csharp
// system-test/Core/Clients/Api/Controllers/OrderControllerClient.cs
var orderClient = new OrderControllerClient(baseUrl);
var response = await orderClient.PlaceOrder(request);
```

## Next Steps

The next evolution steps will be:
- **Step 02**: Extract driver layer for business-level actions
- **Step 03**: Separate channel-specific implementations (API vs UI)
- **Step 04**: Create a domain-specific language (DSL) for tests

## Testing

All tests pass and the build is successful:
```bash
dotnet test
```

## Alignment with Java Version

This implementation follows the same hierarchical pattern as the Java version at:
https://github.com/optivem/atdd-accelerator-eshop-java/tree/step-01-e2e-tests-clients/system-test/src/test/java/com/optivem/atddaccelerator/eshop/systemtest/core/clients

**Structure Mapping:**
- Java: `core/clients/api/` ? .NET: `Core/Clients/Api/`
- Java: `core/clients/ui/` ? .NET: `Core/Clients/Ui/`
- Java: `api/controllers/` ? .NET: `Api/Controllers/`
- Java: `ui/pages/` ? .NET: `Ui/Pages/`
