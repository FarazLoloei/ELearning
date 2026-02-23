# E-Learning Platform API

Production-style backend sample built with .NET 10, Clean Architecture, CQRS, and Domain-Driven Design (DDD).

This repository is intended as **sample code for engineering interviews and job applications**.

## Why This Project Is a Strong Sample

- Clear layer separation (API, Application, Domain, Infrastructure)
- DDD-focused domain model with aggregates and value objects
- Aggregate-root transaction boundary enforcement (no child-entity write repositories)
- CQRS with MediatR and validation pipeline
- Unit of Work + transaction behavior for command consistency
- Warning-clean build and passing test suite

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- MediatR
- FluentValidation
- EF Core (SQL Server)
- GraphQL
- Dapr (read-model integrations)
- xUnit v3

## Solution Structure

```text
ELearning.API                # REST + GraphQL host, middleware, auth, versioning
ELearning.Application        # Commands/queries, handlers, DTOs, behaviors, interfaces
ELearning.Domain             # Aggregates, entities, value objects, domain rules/events
ELearning.Infrastructure     # EF repositories, read-model services, external implementations
ELearning.Persistence        # Persistence project
ELearning.SharedKernel       # Shared abstractions and base types
ELearning.Application.Tests  # Application/unit validation tests
ELearning.IntegrationTests   # API integration tests (WebApplicationFactory)
```

## Architecture and DDD Notes

### Clean Architecture dependency direction

- API -> Application
- Infrastructure -> Application + Domain
- Application -> Domain + SharedKernel
- Domain -> SharedKernel

### DDD boundaries

- Value objects (`Email`, `Duration`, `Rating`) are not persisted independently and have no repositories.
- Minimum transactional consistency is enforced at aggregate-root level.
- Command writes are coordinated through Unit of Work.
- Child entities are accessed for reads only; write orchestration happens through root aggregates.

## Key Functional Areas

- Authentication (`/api/auth/*`)
- Course and instructor flows
- Enrollment lifecycle
- Submission creation and grading
- Student progress reporting

## Configuration

Main configuration files:

- `ELearning.API/appsettings.json`
- `ELearning.API/appsettings.Development.json`

Important settings:

- `ConnectionStrings:DefaultConnection`
- `JwtSettings:Issuer`
- `JwtSettings:Audience`
- `JwtSettings:Secret`
- `JwtSettings:ExpiryInDays`
- `Dapr:HttpPort`
- `Dapr:GrpcPort`

## Run

```powershell
dotnet run --project ELearning.API/ELearning.API.csproj
```

Endpoints:

- REST: `/api/*`
- GraphQL: `/graphql`
- Swagger UI: enabled in Development

## Build and Test

Build:

```powershell
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
```

Run all tests:

```powershell
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

Run only application tests:

```powershell
dotnet test ELearning.Application.Tests/ELearning.Application.Tests.csproj -nologo /p:UseSharedCompilation=false
```

Run only integration tests:

```powershell
dotnet test ELearning.IntegrationTests/ELearning.IntegrationTests.csproj -nologo /p:UseSharedCompilation=false
```

## Testing Approach

- `ELearning.Application.Tests`: DTO and validation-focused tests with shared helpers/factories.
- `ELearning.IntegrationTests`: API-level tests with controlled service replacement via `WebApplicationFactory`.

## Next Steps

- Add an **Application Facade** layer for complex use-case orchestration (single entrypoints for API/GraphQL).
- Add dedicated **read-model repositories/interfaces** for all query-heavy modules (courses, submissions, instructors, students) to fully standardize CQRS read paths.
- Remove remaining child-entity repository abstractions from Domain and keep only aggregate-root write repositories.
- Introduce **architecture tests** (layer dependency tests) to enforce Clean Architecture rules in CI.
- Expand integration tests to cover enrollment and submission workflows end-to-end.
- Add domain-focused unit tests for aggregate invariants (`Course`, `Enrollment`, `User`).
- Add optimistic concurrency handling (row version / concurrency tokens) for high-contention aggregates.
- Add API versioning strategy docs and backward-compatibility contract tests.
- Add CI quality gates: formatting, analyzers, build, tests, and optional coverage threshold.

## License

MIT (`LICENSE.txt`)
