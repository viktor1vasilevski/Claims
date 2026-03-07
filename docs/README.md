# Insurance Claims API

REST API for managing insurance claims and covers. Built with .NET 9, Clean Architecture, MongoDB and SQL Server.

## Stack

- .NET 9 / ASP.NET Core
- MongoDB (claims + covers)
- SQL Server (audit logs)
- TestContainers (local dev)
- FluentValidation, xUnit, Moq

## Running locally

Make sure Docker Desktop is running, then:
```bash
cd Claims
dotnet run
```

TestContainers handles the databases automatically. Swagger available at `https://localhost:7052/swagger`.

## Project structure
```
Claims.Api            → controllers, middleware
Claims.Application    → services, DTOs, validators
Claims.Domain         → models, enums, interfaces
Claims.Infrastructure → repositories, contexts, background services
```

## Notes

- Cover period cannot exceed 1 year, start date cannot be in the past
- Claim damage cost capped at 100,000, created date must fall within cover period
- Audit logs are written asynchronously via Channel<T> to avoid blocking HTTP responses
- Premium is computed progressively based on cover type and period length

## Examples

### Create a Cover

**Request:**
```json
POST /Covers
{
  "startDate": "2026-04-01T00:00:00Z",
  "endDate": "2026-12-31T00:00:00Z",
  "type": "Yacht"
}
```

**Response:**
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
POST /Claims
{
  "coverId": "c54b0312-5fcc-4ee9-b968-fd0c9b8f1795",
  "created": "2026-06-01T00:00:00Z",
  "name": "Collision near port",
  "type": "Collision",
  "damageCost": 5000
}
```

**Response:**
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

### Validation Error Example

**Request:**
```json
POST /Covers
{
  "startDate": "2025-01-01T00:00:00Z",
  "endDate": "2025-12-31T00:00:00Z",
  "type": "Yacht"
}
```

**Response:**
```json
{
  "status": 400,
  "errors": {
    "StartDate": ["StartDate cannot be in the past."]
  }
}
```

## Testing
```bash
dotnet test
```

Unit tests cover `ClaimsService`, `CoversService` and `AuditBackgroundService`.

## CI
GitHub Actions runs build and tests on every push and PR to `master`.
