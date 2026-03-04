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
