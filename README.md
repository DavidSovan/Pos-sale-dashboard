# POS Sale Desktop (Avalonia)

An Avalonia (.NET 9) desktop application for point-of-sale (POS) operations. It communicates with a backend API for authentication, product browsing, cart/sale management, checkout, and receipt viewing.

## Features

- Login with email and password (JWT bearer token stored in-memory).
- Browse product categories and products with pagination and search.
- Start a sale, add/update/remove sale items.
- Checkout with payment method and amount.
- View receipt details after checkout.
- MVVM architecture with ReactiveUI and CommunityToolkit.Mvvm.

## Tech Stack

- .NET 9
- Avalonia 11 (Fluent theme, ReactiveUI)
- CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- System.Net.Http.Json

## Requirements

- .NET SDK 9.x installed
- A running backend API compatible with the following endpoints (default base URL is `http://127.0.0.1:8000`):
  - `POST /api/auth/login`
  - `GET  /api/categories`
  - `GET  /api/products?category_id&search&page`
  - `POST /api/sales` (start a sale)
  - `POST /api/sales/{saleId}/items` (add item)
  - `PATCH /api/sales/{saleId}/items/{itemId}` (update item qty)
  - `DELETE /api/sales/{saleId}/items/{itemId}` (remove item)
  - `PATCH /api/sales/{saleId}/checkout` (checkout)
  - `GET  /api/sales/{saleId}` (receipt)

If your API differs, you will need to adjust the services accordingly.

## Getting Started

### 1) Clone the repository

```bash
git clone https://github.com/DavidSovan/Pos-sale-dashboard.git
cd PosSale
```

### 2) Restore dependencies

```bash
dotnet restore
```

### 3) Configure API base URL (optional)

By default, the app uses:

- File: `Program.cs`
- Code:
  ```csharp
  new HttpClient { BaseAddress = new Uri("http://127.0.0.1:8000") }
  ```

If your backend runs on a different URL, change the `BaseAddress` above to point to your API server. Alternatively, you can add logic to read from an environment variable (e.g., `POSSALE_API_BASEURL`) if you prefer not to hardcode it.

### 4) Run the app

```bash
dotnet run --project PosSale.csproj
```

This will launch the Avalonia desktop application.

### 5) Build (Release)

```bash
dotnet build -c Release
```

### 6) Publish a self-contained build (example: Linux x64)

```bash
dotnet publish -c Release -r linux-x64 --self-contained false -p:PublishSingleFile=true
```

Adjust the runtime identifier (`-r`) for your target platform, e.g. `win10-x64`, `osx-x64`, `osx-arm64`.

## Project Structure

- `App.axaml`, `App.axaml.cs` — Avalonia application and theme setup.
- `Program.cs` — DI container configuration and Avalonia bootstrap; configures `HttpClient` BaseAddress.
- `Services/` — API service clients:
  - `AuthService` — `POST /api/auth/login`, token handling in-memory.
  - `ProductService` — categories, product listing, sale item operations, checkout, receipt fetch.
  - `SaleService` — start sale.
- `Models/` — DTOs for requests/responses (e.g., `CheckoutRequest`, `CheckoutResponse`, etc.).
- `ViewModels/` — ViewModel logic (e.g., `LoginViewModel`, `HomeViewModel`, `SaleViewModel`, `ReceiptViewModel`).
- `Views/` — Avalonia views (`*.axaml` + code-behind).
- `Converters/` — Value converters for binding.
- `Themes/` — Fluent theme customization (e.g., `DarkBlueTheme.axaml`).

## Configuration Details

- API base URL: change in `Program.cs` where the `HttpClient` is registered.
- Authentication: `AuthService` stores the access token in memory only. If you need persistence across restarts, implement secure storage integration and update the token retrieval in services before calling API endpoints.

## Usage Flow (Typical)

1. Login using valid API credentials.
2. Browse categories and products.
3. Start a sale.
4. Add products to the sale (adjust quantities as needed).
5. Checkout with payment method and amount.
6. View/print the receipt.

## Troubleshooting

- "Cannot connect to server" or network errors:
  - Ensure your backend API is running and accessible at the configured `BaseAddress`.
  - Update `Program.cs` to match your backend URL.
- Unauthorized (401):
  - Verify your login credentials.
  - Ensure token is being set on requests (handled in services via `Authorization: Bearer <token>` header).
- Products not showing in UI:
  - Confirm the API returns results and that `Products.Count` is greater than zero in the ViewModel.
  - The project uses a `ListBox` with an `ItemTemplate` (see `Views/SaleView.axaml`) for product display.

## Development Notes

- Packages are declared in `PosSale.csproj`:
  - Avalonia 11.3.3, Avalonia.ReactiveUI 11.3.3, CommunityToolkit.Mvvm 8.2.1, etc.
- Target framework: `net9.0`.
- Logging: basic console logging on startup failure in `Program.cs`.
- Error handling: service methods wrap exceptions with additional context for easier diagnostics.
