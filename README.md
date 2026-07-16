<div align="center">

<br/>

```
  ███████╗██████╗  ██████╗  ██████╗ ███████╗████████╗
  ██╔════╝██╔══██╗██╔═══██╗██╔═══██╗██╔════╝╚══██╔══╝
  █████╗  ██████╔╝██║   ██║██║   ██║███████╗   ██║
  ██╔══╝  ██╔══██╗██║   ██║██║   ██║╚════██║   ██║
  ███████╗██████╔╝╚██████╔╝╚██████╔╝███████║   ██║
  ╚══════╝╚═════╝  ╚═════╝  ╚═════╝ ╚══════╝   ╚═╝
                                          · Backend API
```

**Enterprise-grade e-commerce API — built clean, deployed to the cloud.**

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Azure](https://img.shields.io/badge/Azure-App_Service-0078D4?style=flat-square&logo=microsoftazure&logoColor=white)](https://azure.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF_Core-Code--First-512BD4?style=flat-square&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![CI/CD](https://img.shields.io/badge/GitHub_Actions-CI%2FCD-2088FF?style=flat-square&logo=githubactions&logoColor=white)](../../actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-22C55E?style=flat-square)](./LICENSE)

[API Docs (Swagger)](#) · [Frontend Repo](#) · [Report a Bug](../../issues)

<br/>

</div>

---

## What is EBoost Backend?

The EBoost Backend is the engine behind the EBoost e-commerce platform. It exposes a secure, scalable RESTful API that handles everything — authentication, product management, cart & checkout, Stripe payments, and webhook processing — all structured around **Clean Architecture** for long-term maintainability and testability.

> Already deployed to **Microsoft Azure** with a fully automated **GitHub Actions CI/CD pipeline**.

<br/>

## Architecture

The solution follows a strict four-layer Clean Architecture pattern where dependencies only point inward — never outward.

```
┌──────────────────────────────────────────────────────┐
│                   EBoost.Api                         │  ← Controllers, Middleware, DI, Swagger
├──────────────────────────────────────────────────────┤
│              EBoost.Infrastructure                   │  ← EF Core, Repositories, Stripe, Cloudinary
├──────────────────────────────────────────────────────┤
│               EBoost.Application                     │  ← DTOs, Interfaces, Services, Use Cases
├──────────────────────────────────────────────────────┤
│                 EBoost.Domain                        │  ← Entities, Value Objects, Domain Rules
└──────────────────────────────────────────────────────┘
         Zero external dependencies at the core ✓
```

Each layer only knows about the layer directly below it — no shortcuts, no spaghetti.

<br/>

## Tech Stack

```
Runtime         ASP.NET Core 8 · C# 12+
ORM             Entity Framework Core (Code-First)
Database        Azure SQL Database · Microsoft SQL Server
Auth            JWT · HttpOnly Cookies · Refresh Token Rotation
Payments        Stripe API · Webhook Processing
Image Storage   Cloudinary
Docs            Swagger / OpenAPI
CI/CD           GitHub Actions → Azure App Service
```

<br/>

## Key Features

| Feature | Details |
|---|---|
| 🔐 **Auth & Authorization** | JWT issuance, refresh token rotation, role-based access (Admin / Customer) |
| 📦 **Product Management** | Full CRUD with multi-image upload via Cloudinary |
| 🛒 **Cart & Checkout** | Cart state management flowing into Stripe checkout sessions |
| ⚡ **Webhook Processing** | Async Stripe webhook listener — verifies payments, updates order status automatically |
| 🛡️ **Security** | BCrypt password hashing, HttpOnly cookies, SQL injection prevention, data sanitization |
| ❌ **Global Error Handling** | Custom middleware returning standardized RFC 7807 problem details |

<br/>

## Cloud & CI/CD

```
Push to main
     │
     ▼
GitHub Actions
  ├── Build & compile
  ├── Run tests
  └── Deploy → Azure App Service (zero downtime)
                    │
                    ├── Azure SQL Database
                    │     └── Automated backups · Instant failover
                    └── Cloudinary
                          └── Product image storage
```

Every push to `main` automatically ships to production — no manual deployments needed.

<br/>

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or Docker)
- [Stripe account](https://stripe.com/) — for API keys
- [Cloudinary account](https://cloudinary.com/) — for image hosting

### 1 · Clone & navigate

```bash
git clone https://github.com/your-username/EBoostCleanBackend.git
cd EBoostCleanBackend
```

### 2 · Configure secrets

In `EBoost.Api`, create or update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EBoost;Trusted_Connection=True;Encrypt=False"
  },
  "JwtSettings": {
    "Secret": "your-super-secret-256-bit-key",
    "Issuer": "EBoost",
    "Audience": "EBoostClient"
  },
  "StripeSettings": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "Cloudinary": {
    "CloudName": "...",
    "ApiKey": "...",
    "ApiSecret": "..."
  }
}
```

> 💡 **Tip:** Use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) in development to avoid committing sensitive values.

### 3 · Apply migrations

```bash
dotnet ef database update \
  --project EBoost.Infrastructure \
  --startup-project EBoost.Api
```

### 4 · Run

```bash
dotnet run --project EBoost.Api
```

API live at `https://localhost:5001`  
Swagger UI at `https://localhost:5001/swagger` 🎉

<br/>

## Security Practices

- 🔑 Passwords hashed with **BCrypt** — never stored in plain text
- 🍪 Tokens sent via **HttpOnly cookies** where possible, eliminating XSS via localStorage
- 🧹 All inputs sanitized against SQL injection and over-posting attacks
- 🔄 **Refresh token rotation** — old tokens are invalidated on every refresh cycle

<br/>

## Contributing

Contributions are welcome!

1. Fork the repo
2. Create a branch: `git checkout -b feat/your-feature`
3. Commit: `git commit -m 'feat: describe your change'`
4. Push and open a Pull Request


<br/>

---Deployed on Azure · Built with ☕ by <a href="https://github.com/your-username"></a></sub>
</div>
