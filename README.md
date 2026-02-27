Compliance Transfer Workflow API

A secure, scalable .NET 8 Web API that implements a structured compliance approval workflow with JWT authentication, role-based authorization, and full audit tracking.

✅ Suggested Project Structure

Use this structure (it matches your current folders and is market-standard for a mid-size .NET API):



# Compliance Transfer Workflow API

A secure .NET 8 Web API that implements a state-driven compliance workflow for transfer requests, including JWT authentication, role-based authorization, approval lifecycle handling, and audit trail tracking.

---

## What this project does

In many regulated environments (finance/compliance), users cannot directly complete sensitive transfers. Instead, they raise a request, submit it for review, and an authorized approver decides whether to approve or reject it. Every action must be traceable for audit purposes.

This API models that exact workflow end-to-end:
- Users create transfer requests
- Requests move through controlled states
- Admins approve/reject with comments
- All actions are recorded in `AuditEvents`

---

## Workflow lifecycle

A transfer request follows a strict lifecycle:

**Draft → InReview → Approved / Rejected**

Rules enforced by the service layer:
- Only **Draft** requests can be **Submitted**
- Only **InReview** requests can be **Approved** or **Rejected**
- Invalid transitions return **400 Bad Request**
- Missing/invalid token returns **401 Unauthorized**
- Valid token but wrong role returns **403 Forbidden**

---

## Tech Stack

- **.NET 8 Web API**
- **C#**
- **SQL Server**
- **Dapper** (data access)
- **JWT Bearer Authentication**
- **Swagger / OpenAPI**

---

## Database schema

Your SQL Server database contains these tables:

- `dbo.Users`  
  Stores login users and roles (`Admin` / `User`)

- `dbo.TransferRequests`  
  Stores workflow entity (title, recipient, purpose, status, riskLevel, timestamps)

- `dbo.AuditEvents`  
  Stores audit logs for each request (action, actor, timestamp, comments)

This schema enables both workflow enforcement and full traceability.

---

## API Endpoints

### Auth
- **POST** `/api/auth/login`  
  Logs in and returns JWT token.

### Transfers
- **POST** `/api/transfers`  
  Creates a new request (Draft).
- **GET** `/api/transfers`  
  Lists all requests.
- **POST** `/api/transfers/{id}/submit`  
  Submits Draft request → InReview.

### Approvals (Admin only)
- **POST** `/api/transfers/{id}/approve`  
  Approves request (only if InReview).
- **POST** `/api/transfers/{id}/reject`  
  Rejects request (only if InReview).

### Audit
- **GET** `/api/transfers/{id}/audit`  
  Returns audit history for a request.

---

## Authentication flow

1. Call login endpoint:

**POST** `/api/auth/login`

