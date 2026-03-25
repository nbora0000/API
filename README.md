# 🛍️ Microservices in C# — Order API & Payment API

Two production-ready **ASP.NET Core 8** microservices backed by **MSSQL** via **Entity Framework Core 8**.

## Services

| Service | Port | Swagger UI | Database |
|---------|------|------------|----------|
| **Order API** | 5001 | http://localhost:5001/swagger | `OrdersDb` |
| **Payment API** | 5002 | http://localhost:5002/swagger | `PaymentsDb` |
| **Catalog API** | 5003 | http://localhost:5003/swagger | `CatalogDb` |
| **Basket API** | 5004 | http://localhost:5004/swagger | `BasketDb` |

---

## Quick Start

### Option A — Docker Compose (recommended)

> Requires Docker Desktop

```bash
docker-compose up --build
```

This starts **SQL Server 2022**, **Order API**, and **Payment API** together.  
Databases are auto-created on first startup via EF Core migrations.

### Option B — Run Locally (requires MSSQL installed)

1. Start MSSQL locally on port 1433 (SA password: `YourStrong!Passw0rd`)
2. Update connection strings in `appsettings.json` if needed
3. Run each service:

```bash
# Terminal 1
cd OrderApi
dotnet run

# Terminal 2
cd PaymentApi
dotnet run

# Terminal 3
cd CatalogApi
dotnet run

# Terminal 4
cd BasketApi
dotnet run
```

---

## API Reference

### Order API — `http://localhost:5001/api/orders`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | List orders (filter by `status`, paginate with `page` / `pageSize`) |
| GET | `/api/orders/{id}` | Get a single order |
| POST | `/api/orders` | Create a new order |
| PUT | `/api/orders/{id}` | Update order (status, notes, customer name) |
| DELETE | `/api/orders/{id}` | Delete a Pending or Cancelled order |
| GET | `/health` | Health check |

**Create Order example:**
```json
POST /api/orders
{
  "customerName": "Alice Smith",
  "customerEmail": "alice@example.com",
  "currency": "USD",
  "items": [
    { "productId": "P001", "productName": "Widget", "quantity": 2, "unitPrice": 19.99 }
  ]
}
```

---

### Payment API — `http://localhost:5002/api/payments`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/payments` | List all payments (paginated) |
| GET | `/api/payments/{id}` | Get payment by ID |
| GET | `/api/payments/order/{orderId}` | All payments for an order |
| POST | `/api/payments/process` | Process a payment |
| PUT | `/api/payments/{id}/refund` | Refund a completed payment |
| GET | `/health` | Health check |

**Process Payment example:**
```json
POST /api/payments/process
{
  "orderId": "<order-guid>",
  "amount": 39.98,
  "currency": "USD",
  "method": 0,
  "paymentToken": "tok_visa_test"
}
```

**Payment Methods:** `0=CreditCard`, `1=DebitCard`, `2=BankTransfer`, `3=DigitalWallet`, `4=CashOnDelivery`

> **Note:** `CashOnDelivery` always succeeds. Other methods require a non-empty `paymentToken`.

---

### Catalog API — `http://localhost:5003/api/products`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/products` | List products (filter by `category`) |
| GET | `/api/products/{id}` | Get product by ID |
| POST | `/api/products` | Create a new product |
| PUT | `/api/products/{id}` | Update product |
| DELETE | `/api/products/{id}` | Delete product |
| GET | `/health` | Health check |

---

### Basket API — `http://localhost:5004/api/basket`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/basket/{customerId}` | Get user's basket |
| POST | `/api/basket` | Update user's basket items |
| DELETE | `/api/basket/{customerId}` | Clear user's basket |
| GET | `/health` | Health check |

---

## Project Structure

```
API/
├── Microservices.sln
├── docker-compose.yml
├── SharedLibrary/
│   ├── Enums/
│   │   ├── OrderStatus.cs
│   │   └── PaymentStatus.cs
│   └── Events/
│       └── OrderPlacedEvent.cs
├── OrderApi/
│   ├── Controllers/OrdersController.cs
│   ├── Data/OrderDbContext.cs
│   ├── DTOs/OrderDtos.cs
│   ├── Migrations/
│   ├── Models/Order.cs & OrderItem.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
└── PaymentApi/
    ├── Controllers/PaymentsController.cs
    ├── Data/PaymentDbContext.cs
    ├── DTOs/PaymentDtos.cs
    ├── Migrations/
    ├── Models/Payment.cs
    ├── Dockerfile
    ├── Program.cs
    └── appsettings.json
├── CatalogApi/
│   ├── Controllers/ProductsController.cs
│   ├── Data/CatalogDbContext.cs
│   ├── Models/Product.cs
│   ├── Dockerfile
│   ├── Program.cs
│   └── appsettings.json
└── BasketApi/
    ├── Controllers/BasketController.cs
    ├── Data/BasketDbContext.cs
    ├── Models/ShoppingCart.cs & CartItem.cs
    ├── Dockerfile
    ├── Program.cs
    └── appsettings.json
```

---

## Order Status Flow

```
Pending → Confirmed → Processing → Shipped → Delivered
                                ↘           ↗
                              Cancelled → Refunded
```

## Payment Status Flow

```
Pending → Processing → Completed → Refunded
                    ↘
                    Failed / Cancelled
```

---

## Migrations

To add a new migration after model changes:

```bash
# Order API
dotnet ef migrations add <MigrationName> --project OrderApi

# Payment API
dotnet ef migrations add <MigrationName> --project PaymentApi
```
