# E-Learning Platform API

Clean Architecture + DDD sample for an e-learning platform built on .NET 10.

The solution exposes:
- REST API controllers
- GraphQL endpoint
- CQRS/MediatR application layer
- EF Core write model
- Dapr-based read model clients

## Solution Structure

```text
ELearning.API                # API host (REST, GraphQL, middleware, auth, Ocelot)
ELearning.Application        # Use cases, DTOs, validators, behaviors, interfaces
ELearning.Domain             # Aggregates, entities, value objects, domain events, domain exceptions
ELearning.Infrastructure     # Repositories, services, Dapr read-service implementations
ELearning.Persistence        # Persistence project
ELearning.SharedKernel       # Base abstractions (BaseEntity, ValueObject, Enumeration, etc.)
ELearning.Application.Tests  # Unit/validation tests (xUnit v3)
ELearning.IntegrationTests   # API integration tests (xUnit v3 + WebApplicationFactory)
```

## Architecture

- **Domain layer**: pure business rules and invariants
- **Application layer**: commands/queries, handlers, DTOs, validation pipeline, abstractions
- **Infrastructure layer**: EF Core repositories and external service implementations
- **API layer**: transport concerns (REST/GraphQL), auth middleware, serialization, versioning

Dependency direction follows Clean Architecture rules:
- API -> Application
- Infrastructure -> Application + Domain
- Application -> Domain + SharedKernel
- Domain -> SharedKernel

## Key Features

- Course, module, lesson, assignment, enrollment, submission flows
- JWT authentication endpoints
- FluentValidation pipeline behavior
- Read-model fallback policy for query handlers
- DTO validation tests with helper/factory utilities
- Integration test bootstrapping through `WebApplicationFactory`

## Prerequisites

- .NET SDK 10.0+
- SQL Server (or LocalDB) for full runtime scenarios
- Optional: Dapr runtime for read-model endpoints that depend on Dapr services

## Configuration

Main API settings are loaded from:
- `ELearning.API/appsettings.json`
- `ELearning.API/appsettings.Development.json`
- environment variables

Important keys:
- `ConnectionStrings:DefaultConnection`
- `JwtSettings:Issuer`
- `JwtSettings:Audience`
- `JwtSettings:Secret`
- `JwtSettings:ExpiryInDays`
- `Dapr:HttpPort`
- `Dapr:GrpcPort`

## Run the API

```powershell
dotnet run --project ELearning.API/ELearning.API.csproj
```

Default endpoints:
- REST controllers under `/api/*`
- GraphQL at `/graphql`
- Swagger UI in Development environment

## Build and Test

Build solution:

```powershell
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
```

Run all tests:

```powershell
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

Run application tests only:

```powershell
dotnet test ELearning.Application.Tests/ELearning.Application.Tests.csproj -nologo /p:UseSharedCompilation=false
```

Run integration tests only:

```powershell
dotnet test ELearning.IntegrationTests/ELearning.IntegrationTests.csproj -nologo /p:UseSharedCompilation=false
```

## Testing Approach

- `ELearning.Application.Tests`:
  - DTO and validation-focused tests
  - shared helper patterns to avoid duplicated assertion logic
- `ELearning.IntegrationTests`:
  - API-level integration tests using `WebApplicationFactory`
  - service replacement for controlled test doubles (for deterministic behavior)

## Notes for Sample-Code Use

Current baseline:
- Solution builds successfully
- Tests pass successfully
- Warning-clean baseline maintained in recent updates

If you present this repository as a sample, keep CI validating:
- `dotnet build`
- `dotnet test`

## License

MIT - see `LICENSE.txt`.
