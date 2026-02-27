# Compliance Transfer Workflow API

A secure .NET 8 Web API implementing a state-driven compliance approval workflow with JWT authentication, role-based authorization, and full audit tracking.

---

## Quickstart (one-shot)

1. Clone repository
   - git clone https://github.com/panthati84-ux/ComplianceTransferMini.git
   - cd ComplianceTransferMini/ComplianceTransferMini.API

2. Create the database
   - Use `db/init.sql` to create schema and seed users.
   - Example using `sqlcmd`:
     - `sqlcmd -S localhost -U sa -P "<YourStrong!Passw0rd>" -i db\init.sql`
   - Or run the script in SQL Server Management Studio (SSMS).

3. Configure appsettings
   - Update `appsettings.json` connection string and JWT settings (example below).

4. Run the API
   - Dotnet: `dotnet restore` → `dotnet build` → `dotnet run --project ComplianceTransferMini.API`
   - Visual Studio 2022:
     - Right-click project → __Set as Startup Project__
     - Start: __Debug > Start Debugging__ (F5) or __Debug > Start Without Debugging__ (Ctrl+F5)

5. Open Swagger
   - https://localhost:7170/swagger

6. (Optional) Dockerized SQL Server
   - `docker-compose up -d` (inspect `docker-compose.yml` for service names)

---

## Minimal `appsettings.json` snippet

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ComplianceTransferDb;User Id=sa;Password=<YourStrong!Passw0rd>;"
  },
  "Jwt": {
    "Key": "superSecretKey123!",
    "Issuer": "https://localhost:7170",
    "Audience": "https://localhost:7170"
  }
}
```

---

## Prerequisites

.NET 8 SDK

SQL Server (local) OR SQL Server in Docker

Visual Studio 2022

## Steps

Clone repo

Create DB + tables using db/init.sql

Update appsettings.json connection string

Run the API

Open Swagger:
https://localhost:7170/swagger

Cross-cutting concerns implemented
Global Exception Middleware

Centralizes error handling so controllers stay clean. Converts exceptions into consistent JSON error responses and correct HTTP status codes.

Correlation ID Middleware

Adds a unique correlation ID per request (response header: x-correlation-id) to support debugging and traceability.

## Tip: In Swagger use the Authorize button and paste `Bearer <JWT_TOKEN>`.

---

## API overview

Auth
- POST `/api/auth/login` — Returns JWT token.

Transfers
- POST `/api/transfers` — Create a request (Draft).
- GET `/api/transfers` — List requests.
- POST `/api/transfers/{id}/submit` — Submit Draft → InReview.

Approvals (Admin only)
- POST `/api/transfers/{id}/approve` — Approve (InReview → Approved).
- POST `/api/transfers/{id}/reject` — Reject (InReview → Rejected).

Audit
- GET `/api/transfers/{id}/audit` — Audit history for a request.

---

## Expected HTTP status matrix (common cases)

| Case | Status |
|---|---:|
| Missing / invalid token | 401 Unauthorized |
| Valid token but insufficient role | 403 Forbidden |
| Invalid workflow transition | 400 Bad Request |
| Non-existent request | 404 Not Found |
| Successful create/submit/approve/reject | 200 / 201 OK |

---

## Testing checklist

Positive
- Login as Admin and obtain token
- Create transfer (Draft)
- Submit transfer (InReview)
- Approve / Reject (Approved/Rejected)
- Verify audit entries

Negative
- Login with wrong password → 401
- Call protected endpoints without token → 401
- Approve Draft request → 400
- User role calling approve endpoint → 403
- Invalid requestId → 404

---

## Development tips

- Add Serilog for structured logging.
- Add unit tests with xUnit and integration tests using Testcontainers for SQL Server.
- Use a secret store for production JWT keys and DB credentials.
- Consider adding a Postman collection or OpenAPI client generation for automation.

---

## Author

Praveen Anthati — .NET Full Stack Developer
