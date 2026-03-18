# Insurance Claims API

REST API for managing insurance claims and covers. Built with .NET 9, Clean Architecture, MongoDB and SQL Server.

## Stack

- .NET 9 / ASP.NET Core
- MongoDB (claims + covers)
- SQL Server (audit logs)
- TestContainers (local dev + integration tests)
- FluentValidation, xUnit, Moq

## Project structure

```
Claims.Api            → controllers, middleware, program entry point
Claims.Application    → services, strategies, DTOs, validators
Claims.Domain         → models, enums, interfaces, exceptions
Claims.Infrastructure → repositories, EF Core contexts, background services
```

## Environments

| Environment | Purpose | Databases |
|---|---|---|
| Development | Local development | TestContainers auto-start |
| Staging | Pre-production / CI deploy | Real DBs via secrets |
| Production | Live | Real DBs via secrets |

Each environment has a corresponding `appsettings.{Environment}.json`. Connection strings are never stored in config files — they are injected as environment variables by the pipeline or a secrets manager.

## Running locally

Make sure Docker Desktop is running, then:

```bash
cd Claims
dotnet run
```

TestContainers spins up SQL Server and MongoDB automatically. Swagger is available at `https://localhost:7052/swagger`.

## Testing

### Unit tests

```bash
dotnet test Claims.Application.Tests/Claims.Application.Tests.csproj
dotnet test Claims.Infrastructure.Tests/Claims.Infrastructure.Tests.csproj
```

Cover services, validators, premium calculation strategies, and background service logic.

### Integration tests

```bash
dotnet test Claims.Integration.Tests/Claims.Integration.Tests.csproj
```

End-to-end tests against real databases via TestContainers. The test factory sets `ASPNETCORE_ENVIRONMENT=Staging` and injects connection strings, so they run against the Staging configuration without requiring any manual setup.

## CI/CD pipeline

GitHub Actions runs on every push and PR to `master`:

```
test → deploy-development → deploy-staging → deploy-production
```

| Stage | Trigger | Environment |
|---|---|---|
| test | every push / PR | runs unit + integration tests |
| deploy-development | push to master | `ASPNETCORE_ENVIRONMENT=Development` |
| deploy-staging | after development | `ASPNETCORE_ENVIRONMENT=Staging` |
| deploy-production | after staging | `ASPNETCORE_ENVIRONMENT=Production` |

Deploy stages use GitHub Environments, allowing per-environment secrets and optional manual approval gates before production.

## Business rules

- Cover period cannot exceed 1 year, start date cannot be in the past
- Claim damage cost capped at 100,000, created date must fall within the cover period
- Audit logs written asynchronously via `Channel<T>` to avoid blocking HTTP responses
- Premium computed progressively based on cover type and number of days

## Examples

### Create a Cover

**Request:**
```json
POST /covers
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

### Create a Claim

**Request:**
```json
POST /claims
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
```json
POST /covers
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
