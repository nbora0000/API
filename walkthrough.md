# Microservices Walkthrough — Order API & Payment API

## ✅ Build Status

```
Build succeeded in 2.8s
  SharedLibrary  net8.0  ✅
  OrderApi       net8.0  ✅  → OrderApi\bin\Debug\net8.0\OrderApi.dll
  PaymentApi     net8.0  ✅  → PaymentApi\bin\Debug\net8.0\PaymentApi.dll
```

---

## What Was Built

### Project Structure
```
c:\Users\nbora\source\repos\API\
├── Microservices.sln
├── docker-compose.yml
├── README.md
├── SharedLibrary/           ← Common enums & events
│   ├── Enums/
│   │   ├── OrderStatus.cs   (Pending→Delivered/Cancelled/Refunded)
│   │   └── PaymentStatus.cs (Pending→Completed/Failed/Refunded)
│   └── Events/
│       └── OrderPlacedEvent.cs
├── OrderApi/                ← Port 5001
│   ├── Models/Order.cs + OrderItem.cs
│   ├── Data/OrderDbContext.cs
│   ├── DTOs/OrderDtos.cs
│   ├── Controllers/OrdersController.cs
│   ├── Migrations/InitialCreate
│   ├── Dockerfile
│   └── Program.cs
└── PaymentApi/              ← Port 5002
    ├── Models/Payment.cs
    ├── Data/PaymentDbContext.cs
    ├── DTOs/PaymentDtos.cs
    ├── Controllers/PaymentsController.cs
    ├── Migrations/InitialCreate
    ├── Dockerfile
    └── Program.cs
```

---

## How to Run

### With Docker Compose
```bash
cd c:\Users\nbora\source\repos\API
docker-compose up --build
```
Starts SQL Server 2022 + both APIs. Databases auto-created via EF migrations.

### Locally (MSSQL already installed)
```powershell
# Terminal 1
cd c:\Users\nbora\source\repos\API\OrderApi
dotnet run

# Terminal 2
cd c:\Users\nbora\source\repos\API\PaymentApi
dotnet run
```

---

## API Endpoints

### Order API (http://localhost:5001)

| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/orders | List with filter & pagination |
| GET | /api/orders/{id} | Get by ID |
| POST | /api/orders | Create order |
| PUT | /api/orders/{id} | Update order/status |
| DELETE | /api/orders/{id} | Delete (Pending/Cancelled only) |
| GET | /health | DB health check |
| GET | /swagger | Swagger UI |

### Payment API (http://localhost:5002)

| Method | Route | Description |
|--------|-------|-------------|
| GET | /api/payments | List with filter & pagination |
| GET | /api/payments/{id} | Get by ID |
| GET | /api/payments/order/{orderId} | Payments for an order |
| POST | /api/payments/process | Process a payment |
| PUT | /api/payments/{id}/refund | Refund a payment |
| GET | /health | DB health check |
| GET | /swagger | Swagger UI |

---

## Key Design Decisions

- **Database per service**: `OrdersDb` and `PaymentsDb` are completely isolated
- **Auto-migration**: `db.Database.Migrate()` on startup — no manual migration step needed
- **Business rules**: Cannot update Delivered/Cancelled orders; cannot refund non-Completed payments
- **Simulated gateway**: `CashOnDelivery` auto-succeeds, other methods need a `paymentToken`
- **Retry on failure**: EF Core configured with `EnableRetryOnFailure(5)` for transient faults
