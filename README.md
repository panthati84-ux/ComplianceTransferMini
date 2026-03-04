# ComplianceTransferMini.API

A secure **.NET 8 Web API** that implements a **compliance transfer approval workflow** with authentication, role-based authorization, and audit tracking.

This project demonstrates how to build a **production-style backend API** using ASP.NET Core with layered architecture, JWT authentication, and repository/service patterns.

---

# 🚀 Technologies Used

- .NET 8
- ASP.NET Core Web API
- JWT Authentication
- Dapper
- Entity Framework Core
- SQL Server
- Swagger / OpenAPI
- Dependency Injection
- Repository Pattern

---

# 📂 Project Structure

```
ComplianceTransferMini.API
│
├── Controllers
│   ├── AuthController.cs
│   ├── TransfersController.cs
│   ├── ApprovalsController.cs
│   └── AuditController.cs
│
├── Services
│   ├── AuthService.cs
│   └── TransferService.cs
│
├── Repositories
│   ├── UserRepository.cs
│   ├── TransferRepository.cs
│   └── AuditRepository.cs
│
├── Models
│   ├── User.cs
│   ├── TransferRequest.cs
│   └── AuditLog.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── Helpers
│   └── JwtTokenHelper.cs
│
├── Program.cs
└── appsettings.json
```

---

# 📊 Key Features

- User authentication with JWT
- Role-based authorization
- Create transfer requests
- Submit transfers for approval
- Approve or reject transfers
- Audit log tracking for compliance
- Secure API endpoints
- Swagger documentation

---

# 🔐 Authentication

Authentication is implemented using **JWT (JSON Web Token)**.

Users must login to obtain a token.

Example request:

```
POST /api/auth/login
```

Example response:

```
{
  "token": "JWT_TOKEN_HERE"
}
```

This token must be included in request headers:

```
Authorization: Bearer {token}
```

---

# 📌 API Endpoints

### Authentication

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /api/auth/login | User login |

---

### Transfer Requests

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /api/transfers | Create transfer request |
| GET | /api/transfers | Get all transfers |
| GET | /api/transfers/{id} | Get transfer details |
| PUT | /api/transfers/{id} | Update transfer |
| DELETE | /api/transfers/{id} | Delete transfer |

---

### Approvals

| Method | Endpoint | Description |
|------|---------|-------------|
| POST | /api/approvals/approve/{id} | Approve transfer |
| POST | /api/approvals/reject/{id} | Reject transfer |

---

### Audit Logs

| Method | Endpoint | Description |
|------|---------|-------------|
| GET | /api/audit/{transferId} | Get audit history |

---

# 🗄 Database Configuration

Connection string is defined in:

```
appsettings.json
```

Example:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=ComplianceTransferDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

---

# ⚙️ Running the Application

### 1 Clone the repository

```
git clone https://github.com/panthati84-ux/ComplianceTransferMini
```

---

### 2 Open the project

Open solution in **Visual Studio**

```
ComplianceTransferMini.API.sln
```

---

### 3 Configure database

Update connection string in:

```
appsettings.json
```

---

### 4 Run migrations (if EF used)

```
Update-Database
```

---

### 5 Run the project

Press

```
F5
```

or run

```
dotnet run
```

---

# 📄 Swagger API Documentation

After running the project, open:

```
https://localhost:xxxx/swagger
```

Swagger provides interactive documentation to test API endpoints.

---

# 🧠 Workflow

1. User logs in
2. User creates transfer request
3. Request is submitted for approval
4. Approver approves/rejects request
5. All actions are logged in audit history

---

# 📚 Learning Purpose

This project demonstrates:

- Secure API development
- JWT authentication
- Repository and service patterns
- Enterprise-style API design
- Compliance workflow implementation

---

# 👨‍💻 Author

**Praveen Anthati**  
.NET Full Stack Developer
