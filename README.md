# NexusShop — Product Microservice

A production-style **Product microservice** for an e-commerce platform, built with
**ASP.NET Core 8** and **Clean Architecture**. The project demonstrates industry
standards in software architecture, security, validation and automated testing.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![Architecture](https://img.shields.io/badge/Architecture-Clean-2ea44f)
![Tests](https://img.shields.io/badge/Tests-xUnit-blueviolet)
![Coverage](https://img.shields.io/badge/Application%20coverage-%3E80%25-success)

---

## Table of contents

- [Highlights](#highlights)
- [Architecture](#architecture)
- [Tech stack](#tech-stack)
- [Getting started](#getting-started)
- [Authentication & roles](#authentication--roles)
- [API reference](#api-reference)
- [Testing](#testing)
- [Project structure](#project-structure)
- [Design decisions](#design-decisions)

---

## Highlights

- **Clean Architecture** with four strictly separated layers and a dependency rule
  that always points inward (Domain has zero external dependencies).
- **Domain-Driven design touches**: rich entities that protect their invariants, a
  `Money` value object, and a domain-specific exception type.
- **EF Core 8, Code-First** with Fluent API configuration and an owned-type mapping
  for the `Money` value object.
- **JWT authentication** with role-based authorization — only `Admin` users may
  create, update or delete products.
- **FluentValidation** for all incoming write models (e.g. price must be `> 0`).
- **Swagger / OpenAPI** with a built-in *Authorize* button to send Bearer tokens.
- **xUnit test suite** using **FluentAssertions** and **Moq**, covering the
  Application-layer business logic to **> 80 %**.
- **Runs anywhere with zero setup** — SQLite is the default provider, so a reviewer
  can clone and press *F5*. Switch to SQL Server with a single config value.

---

## Architecture

The solution follows the Clean Architecture dependency rule: source-code
dependencies only point **inward**. The Domain is the stable core and knows
nothing about EF Core, ASP.NET or any framework.

```
                +--------------------------------------------------+
                |                     WebAPI                        |
                |  Controllers · JWT · Swagger · Middleware         |
                +-----------------------+--------------------------+
                                        |  depends on
                +-----------------------v--------------------------+
                |                 Infrastructure                    |
                |  EF Core · Repositories · DbContext · Seeding     |
                +-----------------------+--------------------------+
                                        |  depends on
                +-----------------------v--------------------------+
                |                  Application                       |
                |  Use-cases (Services) · DTOs · Mapping · Validators|
                +-----------------------+--------------------------+
                                        |  depends on
                +-----------------------v--------------------------+
                |                     Domain                         |
                |  Entities · Value Objects · Repository Interfaces  |
                |              (no external dependencies)            |
                +--------------------------------------------------+
```

**Why interfaces live in Domain:** the repository abstractions
(`IProductRepository`, `IUnitOfWork`, …) are declared in the Domain layer and
*implemented* in Infrastructure. This is the Dependency Inversion Principle in
action — the inner layers define the contracts, the outer layers fulfil them.

---

## Tech stack

| Concern            | Technology                                             |
|--------------------|--------------------------------------------------------|
| Runtime / language | .NET 8, C# 12                                          |
| Web framework      | ASP.NET Core Web API                                   |
| Persistence        | Entity Framework Core 8 (SQL Server / SQLite)          |
| Object mapping     | Explicit mapping via extension methods (no AutoMapper) |
| Validation         | FluentValidation                                       |
| Security           | JWT Bearer authentication, role-based authorization    |
| API documentation  | Swashbuckle (Swagger / OpenAPI)                       |
| Testing            | xUnit · FluentAssertions · Moq · coverlet              |

---

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (17.8+), JetBrains Rider, or VS Code

### Run it

```bash
git clone <your-repo-url>
cd NexusShop

dotnet restore
dotnet run --project src/NexusShop.WebAPI
```

Then open the Swagger UI at **https://localhost:7150/**.

On first start the app creates a local SQLite database (`nexusshop.db`) and seeds
it with demo categories and products — no database installation required.

### Switch to SQL Server

In `src/NexusShop.WebAPI/appsettings.json` set:

```json
"DatabaseProvider": "SqlServer"
```

and adjust the `ConnectionStrings:SqlServer` value. LocalDB works out of the box
on Windows with the default connection string.

---

## Authentication & roles

Two demo users are available (in-memory, for demonstration only):

| Username   | Password       | Role     | Can manage products? |
|------------|----------------|----------|----------------------|
| `admin`    | `Admin123!`    | Admin    | ✅ yes               |
| `customer` | `Customer123!` | Customer | ❌ no                |

**How to call a protected endpoint:**

1. `POST /api/auth/login` with the admin credentials → copy the `accessToken`.
2. In Swagger, click **Authorize** and paste the token.
3. You can now call `POST` / `PUT` / `DELETE` on `/api/products`.

Read endpoints (`GET`) are public; write endpoints require the `Admin` role.

---

## API reference

| Method | Route                              | Auth        | Description                  |
|--------|------------------------------------|-------------|------------------------------|
| POST   | `/api/auth/login`                  | Public      | Obtain a JWT                 |
| GET    | `/api/products`                    | Public      | List all products           |
| GET    | `/api/products/{id}`               | Public      | Get a product by id          |
| GET    | `/api/products/by-category/{id}`   | Public      | List products of a category  |
| POST   | `/api/products`                    | Admin       | Create a product             |
| PUT    | `/api/products/{id}`               | Admin       | Update a product             |
| DELETE | `/api/products/{id}`               | Admin       | Delete a product             |
| GET    | `/api/categories`                  | Public      | List all categories          |
| GET    | `/api/categories/{id}`             | Public      | Get a category by id         |
| POST   | `/api/categories`                  | Admin       | Create a category            |

A ready-to-use request collection is provided in
[`src/NexusShop.WebAPI/NexusShop.WebAPI.http`](src/NexusShop.WebAPI/NexusShop.WebAPI.http).

---

## Testing

```bash
# Run the full test suite
dotnet test

# Run with code coverage (coverlet)
dotnet test --collect:"XPlat Code Coverage"
```

Or use the helper scripts: `./verify.sh` (Linux/macOS) or `pwsh ./verify.ps1` (Windows).

The test project (`NexusShop.Application.Tests`) covers:

- **Service / use-case logic** — happy paths, not-found handling, validation
  failures, and persistence side-effects (verified with Moq).
- **Validators** — boundary cases for price, name, stock, currency and category.
- **Domain invariants** — `Product` and `Money` rules that must never be violated.

Repositories are **mocked** so the Application layer is tested in isolation,
without touching a database. This keeps the business-logic coverage above 80 %.

### Database migrations (optional)

The demo uses `EnsureCreated()` for a frictionless first run. To use proper EF Core
migrations instead:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate \
  --project src/NexusShop.Infrastructure \
  --startup-project src/NexusShop.WebAPI
dotnet ef database update \
  --project src/NexusShop.Infrastructure \
  --startup-project src/NexusShop.WebAPI
```

A `DesignTimeDbContextFactory` is included so the EF CLI works without running the host.

---

## Project structure

```
NexusShop/
├── src/
│   ├── NexusShop.Domain/          # Entities, value objects, repository interfaces
│   ├── NexusShop.Application/     # Use-cases, DTOs, AutoMapper, FluentValidation
│   ├── NexusShop.Infrastructure/  # EF Core, repositories, unit of work, seeding
│   └── NexusShop.WebAPI/          # Controllers, JWT, Swagger, middleware
├── tests/
│   └── NexusShop.Application.Tests/  # xUnit + FluentAssertions + Moq
├── NexusShop.sln
└── README.md
```

---

## Design decisions

- **Owned type for `Money`.** Price and currency are modelled as a single value
  object and persisted into the `Products` table via EF Core's `OwnsOne`, keeping
  the domain expressive without an extra table.
- **Unit of Work over the DbContext.** Services depend on `IUnitOfWork`, so a use-case
  commits all changes in one transaction and stays fully mockable.
- **Exceptions as control flow for cross-cutting concerns.** `NotFoundException`,
  `ValidationException` and `DomainException` are translated to the correct HTTP
  status codes by a single middleware, keeping controllers thin.
- **Provider-agnostic persistence.** The same model runs on SQLite and SQL Server;
  `HasPrecision(18,2)` is used instead of a provider-specific column type.
- **Explicit mapping over AutoMapper.** Entity-to-DTO mapping is done with small,
  compile-time-checked extension methods. This removes a third-party dependency,
  keeps the mappings trivial to read and debug, and avoids a known AutoMapper
  advisory whose fix is only available in the now-commercial releases.

---

> Built as a portfolio project to demonstrate Clean Architecture, secure API design
> and a disciplined testing strategy in the .NET ecosystem.
