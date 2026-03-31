# Insurance Claims API

REST API for managing insurance claims and covers for maritime vessels. Built with .NET 9, Clean Architecture, SQL Server, and MongoDB.

## Stack

- .NET 9 / ASP.NET Core
- **SQL Server** via Entity Framework Core — claims and covers
- **MongoDB** via MongoDB.EntityFrameworkCore — audit logs
- **Azure Service Bus** — async audit messaging (emulated locally via Docker)
- **Polly** — retry pipeline with exponential backoff for MongoDB writes
- FluentValidation, xUnit, Moq, FluentAssertions
- TestContainers — real databases for integration tests
- Swagger / OpenAPI

## Project structure

```
Claims/                    → controllers, middleware, program entry point
Claims.Application/        → services, strategies, validators, DTOs, mappers
Claims.Domain/             → models, enums, interfaces, domain exceptions
Claims.Infrastructure/     → EF Core contexts, repositories, Service Bus messaging, background services

Claims.Api.Tests/          → controller and middleware unit tests
Claims.Application.Tests/  → service, validator, calculator, mapper unit tests
Claims.Domain.Tests/       → domain model and business rule unit tests
Claims.Infrastructure.Tests/ → background service and messaging unit tests
Claims.Integration.Tests/  → end-to-end tests against real TestContainers databases

infra/                     → Terraform modules for Azure (App Service, SQL, Cosmos DB, Service Bus)
docs/                      → this file
```

## Architecture

The solution follows Clean Architecture with four layers:

```
HTTP Request
  → Controller (Claims / Covers)
  → FluentValidation
  → Service (ClaimsService / CoversService)
  → Repository (SQL Server via EF Core)
  → DTO response

  + AuditService (fire-and-forget via Service Bus)
      → AuditBackgroundService (hosted service)
          → Polly retry (3 attempts, exponential backoff)
          → AuditRepository → MongoDB
          → message acknowledged
```

Audit writes are fully decoupled from the HTTP response path. The `AuditMessageEnvelope` pattern defers Service Bus message acknowledgement until after the MongoDB write succeeds, ensuring no audit event is silently dropped.

## Premium calculation

Premiums are computed with a Strategy pattern, one strategy per cover type. All strategies share:

- Base day rate: **1,250** per day
- Progressive discounts after 30 days and 180 days

| Cover type | Multiplier | Discount (days 31–180) | Additional discount (180+) |
|---|---|---|---|
| Yacht | 1.10× | 5% | +3% |
| Tanker | 1.50× | 2% | +1% |
| Passenger ship | 1.30× | 2% | +1% |
| Container ship | 1.30× | 2% | +1% |
| Bulk carrier | 1.30× | 2% | +1% |

Use `GET /covers/premium?startDate=...&endDate=...&coverType=...` to compute a premium without creating a cover.

## Business rules

| Rule | Detail |
|---|---|
| Cover period | Cannot exceed 1 year (365 days); start date cannot be in the past |
| Claim date | Must fall within the cover's start–end period |
| Damage cost | Must be > 0 and ≤ 100,000 |
| Cover deletion | Rejected with `409 Conflict` if any claims reference the cover |

## Environments

| Environment | Purpose | Databases |
|---|---|---|
| Development | Local development | TestContainers auto-start |
| Staging | Pre-production / CI deploy | Real DBs injected via secrets |
| Production | Live | Real DBs injected via secrets |

Connection strings are never stored in config files — they are injected as environment variables by the pipeline or a secrets manager.

## Running locally

Requires Docker Desktop.

### Option A — TestContainers (zero config)

```bash
cd Claims
dotnet run
```

TestContainers automatically pulls and starts SQL Server and MongoDB on first run. The Service Bus emulator is **not** started this way; audit messages fall through gracefully in Development mode.

### Option B — Docker Compose (full stack including Service Bus)

```bash
docker-compose up -d
cd Claims
dotnet run
```

The compose file starts SQL Edge and the Azure Service Bus emulator with the emulator config from `infra/servicebus-emulator-config.json`.

Swagger is available at `https://localhost:7052/swagger`.

## Testing

### All unit tests

```bash
dotnet test --filter "FullyQualifiedName!~Integration"
```

### Individual test projects

```bash
dotnet test Claims.Api.Tests/Claims.Api.Tests.csproj
dotnet test Claims.Application.Tests/Claims.Application.Tests.csproj
dotnet test Claims.Domain.Tests/Claims.Domain.Tests.csproj
dotnet test Claims.Infrastructure.Tests/Claims.Infrastructure.Tests.csproj
```

Cover controllers, middleware, services, validators, premium calculation strategies, domain model rules, and background service logic.

### Integration tests

```bash
dotnet test Claims.Integration.Tests/Claims.Integration.Tests.csproj
```

End-to-end tests that spin up real SQL Server and MongoDB containers via TestContainers. The `ClaimsApiFactory` replaces Azure Service Bus with an in-process `InProcessAuditQueue` so no external dependencies are required.

## CI/CD pipeline

GitHub Actions runs on every push and PR to `master`:

| Stage | Trigger | What it does |
|---|---|---|
| `build` | every push | Restore, build, cache NuGet |
| `unit-tests` | after build | All 4 unit test projects with code coverage |
| `integration-tests` | after build | `Claims.Integration.Tests` with TestContainers |
| `deploy-staging` | push to `master` after tests | Terraform provision + EF migrations + deploy + smoke test |
| `deploy-production` | manual approval after staging | Same as staging against production resources |

Deploy stages use GitHub Environments for per-environment secrets and optional manual approval gates.

## API endpoints

### Covers

| Method | Path | Description |
|---|---|---|
| `GET` | `/covers` | List all covers |
| `GET` | `/covers/{id}` | Get cover by ID |
| `POST` | `/covers` | Create a cover |
| `DELETE` | `/covers/{id}` | Delete a cover (409 if claims exist) |
| `GET` | `/covers/premium` | Compute premium (no cover created) |

### Claims

| Method | Path | Description |
|---|---|---|
| `GET` | `/claims` | List all claims |
| `GET` | `/claims/{id}` | Get claim by ID |
| `POST` | `/claims` | Create a claim |
| `DELETE` | `/claims/{id}` | Delete a claim |

### Health

| Method | Path | Description |
|---|---|---|
| `GET` | `/health` | Liveness probe (checks SQL + MongoDB) |

## Examples

### Create a cover

**Request:**
```
POST /covers
```
```json
{
  "startDate": "2026-04-01T00:00:00Z",
  "endDate": "2026-12-31T00:00:00Z",
  "type": "Yacht"
}
```

**Response `201 Created`:**
```json
{
  "id": "c54b0312-5fcc-4ee9-b968-fd0c9b8f1795",
  "startDate": "2026-04-01T00:00:00Z",
  "endDate": "2026-12-31T00:00:00Z",
  "type": "Yacht",
  "premium": 660935
}
```

### Create a claim

**Request:**
```
POST /claims
```
```json
{
  "coverId": "c54b0312-5fcc-4ee9-b968-fd0c9b8f1795",
  "created": "2026-06-01T00:00:00Z",
  "name": "Collision near port",
  "type": "Collision",
  "damageCost": 5000
}
```

**Response `201 Created`:**
```json
{
  "id": "a06580bc-9af5-4ff9-89d8-5fa7695222d0",
  "coverId": "c54b0312-5fcc-4ee9-b968-fd0c9b8f1795",
  "created": "2026-06-01T00:00:00Z",
  "name": "Collision near port",
  "type": "Collision",
  "damageCost": 5000
}
```

### Validation error

**Request:**
```
POST /covers
```
```json
{
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-12-31T00:00:00Z",
  "type": "Yacht"
}
```

**Response `400 Bad Request`:**
```json
{
  "status": 400,
  "errors": {
    "StartDate": ["StartDate cannot be in the past."]
  }
}
```
