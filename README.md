<div align="center">
  <h1>🚀 EBoost E-Commerce Backend (Clean Architecture)</h1>
  <p>A high-performance, enterprise-grade e-commerce backend API built with ASP.NET Core, completely structured around Clean Architecture principles.</p>
</div>

---

## 🌟 Overview

The **EBoost Backend** serves as the powerhouse for the EBoost E-Commerce application. It provides secure, scalable, and robust RESTful APIs to manage users, products, orders, shopping carts, and payments. 

Designed with strict adherence to **Clean Architecture**, the application enforces a clear separation of concerns, ensuring high maintainability, testability, and independence from external frameworks.

## ☁️ Azure Deployment & CI/CD (High-Availability)

This application is fully containerized/configured for the cloud and is actively deployed on **Microsoft Azure**.

- **App Service**: Hosted on Azure App Service (`eboost-api`), providing auto-scaling, SSL termination, and continuous availability.
- **Database**: Powered by **Azure SQL Database**, offering enterprise-grade security, automated backups, and instant failover capabilities.
- **Continuous Integration/Continuous Deployment (CI/CD)**: 
  - Fully automated deployment pipeline via **GitHub Actions**.
  - Every push to the `main` branch automatically triggers a build, runs tests, and deploys the latest production-ready code directly to the Azure Web App without downtime.

## 🏗️ Clean Architecture Structure

The solution is divided into four highly decoupled layers:

1. **`EBoost.Domain` (Enterprise Business Rules)**
   - Contains Entities, Value Objects, and Domain exceptions.
   - Zero dependencies on other projects or external frameworks.
2. **`EBoost.Application` (Application Business Rules)**
   - Contains DTOs, Interfaces, Service implementations, and Use Cases.
   - Depends only on the Domain layer.
3. **`EBoost.Infrastructure` (Frameworks & Drivers)**
   - Implements Data Access (Entity Framework Core), Repositories, and third-party integrations (Stripe, Cloudinary).
   - Depends on the Application layer to implement its interfaces.
4. **`EBoost.Api` (Presentation / Interface Adapters)**
   - The ASP.NET Core Web API layer.
   - Contains Controllers, Middleware, Dependency Injection setup, and Swagger configurations.

## 🛠️ Technology Stack

* **Framework:** ASP.NET Core 8.0/9.0 Web API
* **Language:** C# 12+
* **ORM:** Entity Framework Core (Code-First Approach)
* **Database:** Azure SQL Database / Microsoft SQL Server
* **Authentication:** JWT (JSON Web Tokens) with HttpOnly Cookie support for enhanced security.
* **Payment Gateway:** Stripe API Integration
* **Image Hosting:** Cloudinary Cloud Storage
* **Documentation:** Swagger (OpenAPI)

## 🔑 Key Features

* **Advanced Authentication & Authorization:** Role-based access control (Admin vs. Customer), secure JWT issuance, and refresh token rotation.
* **Product Management:** Full CRUD capabilities with multi-image support (uploaded securely to Cloudinary).
* **Cart & Checkout Flow:** Seamless shopping cart state management transitioned into Stripe checkout sessions.
* **Webhook Processing:** Secure Stripe Webhook listener to verify payments and automatically update order statuses asynchronously.
* **Global Error Handling:** Custom exception-handling middleware providing standardized API problem details.

## 💻 Getting Started (Local Development)

### Prerequisites
- [.NET SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or Docker container)
- Stripe Account (for API Keys)
- Cloudinary Account (for Image hosting)

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/EBoostCleanBackend.git
   cd EBoostCleanBackend
   ```

2. **Configure User Secrets or `appsettings.Development.json`:**
   Navigate to the `EBoost.Api` directory and set up your environment variables:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=EBoostDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False"
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

3. **Apply Database Migrations:**
   ```bash
   dotnet ef database update --project EBoost.Infrastructure --startup-project EBoost.Api
   ```

4. **Run the API:**
   ```bash
   dotnet run --project EBoost.Api
   ```
   The API will start at `https://localhost:5001`. Navigate to `https://localhost:5001/swagger` to explore the endpoints.

## 🛡️ Security

This project implements modern security best practices:
- Passwords are cryptographically hashed using **BCrypt**.
- Tokens are transmitted securely, avoiding local storage XSS vulnerabilities where possible.
- API endpoints are heavily sanitized to prevent SQL Injection and excessive data exposure.

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request if you'd like to improve the architecture or add features.
