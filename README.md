# E-Learning Platform API

Monolith backend sample built with .NET 10 using Clean Architecture, CQRS, and DDD-style domain modeling.

This repository is curated as portfolio code for senior/backend engineering interviews.

## What This Demonstrates

- Clean layer boundaries across API, Application, Domain, Infrastructure, SharedKernel
- Command/query separation with MediatR pipeline behaviors
- Aggregate-focused domain model with value objects and invariants
- Consistent API envelope responses for REST and a secondary GraphQL surface
- Integration point pattern for external read models (Dapr-style fallback)

## Architecture (Monolith + Clean Architecture)

```text
ELearning.API                # Web host, REST controllers, GraphQL schema/resolvers
ELearning.Application        # Use cases (commands/queries), handlers, DTOs, behaviors
ELearning.Domain             # Aggregates, entities, value objects, domain rules
ELearning.Infrastructure     # EF Core, repositories, auth, read-model services, gateways
ELearning.Persistence        # Persistence project boundary
ELearning.SharedKernel       # Cross-cutting abstractions/base types
ELearning.Application.Tests  # Application validation/unit tests
ELearning.IntegrationTests   # API integration tests
```

Dependency direction (intended):

- API -> Application
- Infrastructure -> Application + Domain
- Application -> Domain + SharedKernel
- Domain -> SharedKernel

## Domain Scope

Primary business flows currently implemented:

- Authentication and role-based access (Student, Instructor, Admin)
- Course lifecycle (create/update/delete/list/filter)
- Enrollment lifecycle and student progress access
- Assignment submission and instructor grading
- Instructor pending-submissions workflow

## Tech Stack

- .NET 10 / ASP.NET Core
- EF Core (Sqlite in-memory by default, SQL Server configurable)
- MediatR + FluentValidation
- HotChocolate GraphQL
- Swagger/OpenAPI
- Ocelot (gateway config present)
- xUnit v3

## Local Run (Quick Start)

1. Configure required settings (at minimum JWT values):

```powershell
dotnet user-secrets set "JwtSettings:Issuer" "elearning-local" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:Audience" "elearning-local" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:Secret" "your-long-random-secret-at-least-32-characters" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:ExpiryInDays" "7" --project ELearning.API/ELearning.API.csproj
```

2. Run API:

```powershell
dotnet run --project ELearning.API/ELearning.API.csproj
```

3. Open endpoints:

- Swagger UI: `/`
- REST: `/api/v1/*` (also `/api/*` compatibility routes)
- GraphQL: `/graphql`

## Build and Test

```powershell
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

Current test status from latest local run:

- `ELearning.Application.Tests`: 71 passed
- `ELearning.IntegrationTests`: 1 passed

## API Notes

- REST is the primary interface for this sample.
- GraphQL is intentionally secondary and reuses the same Application use cases.
- Responses use a consistent envelope (`succeeded`, `data`, `error`) for REST.

## Why This Is A Strong Portfolio Monolith

- Business logic is centered in Application + Domain, not controllers.
- Request validation and transaction handling are centralized via MediatR behaviors.
- The codebase shows clear seams for scaling later (read services, facades, abstractions).
- Test projects are separated by responsibility (application-level vs integration-level).

## Roadmap (High-Impact Next Additions)

- Add global exception-to-problem-details mapping middleware.
- Add architecture tests enforcing layer dependency rules in CI.
- Increase integration coverage to enrollment/submission end-to-end scenarios.
- Add optimistic concurrency (row version tokens) for write-heavy aggregates.
- Add migration-based database bootstrapping for production safety.
- Expand auth story with refresh tokens/revocation and audit events.

## License

MIT (`LICENSE.txt`)
