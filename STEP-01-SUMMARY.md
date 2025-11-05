# Step 01: E2E Tests - Clients Refactoring

## Overview

This step introduces the **Client Layer** pattern to the E2E tests, extracting HTTP and UI interaction logic into dedicated client classes. This follows the **Page Object Model** for UI tests and the **API Client** pattern for API tests.

## Changes Made

### 1. Created Client Classes

#### API Client (`system-test/Clients/ApiClient.cs`)
- Encapsulates all HTTP interactions with the API
- Provides methods: `PlaceOrder()`, `GetOrder()`, `CancelOrder()`, `IsHealthy()`
- Handles JSON serialization/deserialization
- Uses HttpClient with base URL configuration

#### API Models (`system-test/Clients/ApiModels.cs`)
- Extracted DTOs from test classes into shared models
- Classes: `PlaceOrderRequest`, `PlaceOrderResponse`, `GetOrderResponse`

#### Shop Page Client (`system-test/Clients/ShopPageClient.cs`)
- Implements Page Object Model for the Shop page
- Provides methods: `NavigateToShop()`, `FillProductId()`, `FillQuantity()`, `ClickPlaceOrder()`, `GetConfirmationMessage()`
- Includes helper methods: `ParseConfirmationMessage()`, `ExtractOrderNumber()`

#### Order History Page Client (`system-test/Clients/OrderHistoryPageClient.cs`)
- Implements Page Object Model for the Order History page
- Provides methods: `NavigateToOrderHistory()`, `SearchOrder()`, `GetOrderDetails()`, `ClickCancelOrder()`
- Returns structured data via `OrderDetailsDisplay` class

### 2. Refactored Tests

#### API E2E Tests (`system-test/E2eTests/ApiE2eTest.cs`)
- **Before**: Direct HttpClient calls with inline JSON handling
- **After**: Uses `ApiClient` for all API interactions
- **Benefits**: 
  - Tests are more readable and focused on business logic
  - HTTP details are abstracted away
  - Easier to maintain and reuse API calls

#### UI E2E Tests (`system-test/E2eTests/UiE2eTest.cs`)
- **Before**: Direct Playwright locator calls scattered throughout tests
- **After**: Uses `ShopPageClient` and `OrderHistoryPageClient`
- **Benefits**:
  - Tests read like business scenarios
  - Page structure changes only require updates in one place
  - Reusable page interaction methods

### 3. Updated README
- Added **Process** section showing step-by-step evolution
- Links to each branch/step in the learning path

## Benefits of This Refactoring

### 1. **Separation of Concerns**
- Test logic (what to test) is separated from implementation details (how to interact)
- Tests focus on business behavior
- Clients handle technical interactions

### 2. **Reusability**
- Client methods can be reused across multiple tests
- Common interactions are defined once

### 3. **Maintainability**
- If API or UI changes, only clients need updates
- Tests remain stable and focused on behavior
- Reduces duplication

### 4. **Readability**
- Tests are more declarative and easier to understand
- Business intent is clearer
- Less technical noise in test code

### 5. **Testability**
- Clients can be tested independently
- Easier to mock/stub for isolated testing

## Example Comparison

### Before (Direct Interactions)
```csharp
var request = new PlaceOrderRequest { ProductId = 10, Quantity = 5 };
using var client = new HttpClient();
var response = await client.PostAsJsonAsync($"{TestConfiguration.BaseUrl}/api/orders", request);
var responseBody = await response.Content.ReadAsStringAsync();
var orderResponse = JsonSerializer.Deserialize<PlaceOrderResponse>(responseBody, JsonOptions);
```

### After (Using Client)
```csharp
var request = new PlaceOrderRequest { ProductId = 10, Quantity = 5 };
var response = await _apiClient.PlaceOrder(request);
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

This implementation follows the same pattern as the Java version at:
https://github.com/optivem/atdd-accelerator-eshop-java/tree/step-01-e2e-tests-clients
