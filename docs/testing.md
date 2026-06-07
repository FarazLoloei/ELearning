# Testing

The test suite is designed to demonstrate behavior, architecture boundaries, and infrastructure integration without requiring external services for normal test runs.

## Test Projects

| Project | Focus |
| --- | --- |
| `ELearning.Application.Tests` | domain behavior, application services, DTO validation, architecture rules |
| `ELearning.IntegrationTests` | HTTP workflows, authentication/authorization, health checks, configuration, persistence, outbox, notification idempotency |

Run the validation commands below to see the current test totals in your environment.

## Coverage Themes

- course lifecycle and authoring behavior
- enrollment progression and completion rules
- assessment workflow
- certificate issuance
- authorization and role behavior
- REST error contracts
- health endpoints
- configuration validation
- outbox publishing behavior
- notification request and idempotency behavior
- architecture dependency direction

## CI Quality Gates

GitHub Actions runs on pull requests and pushes to `master`.

Pipeline:

- restore
- build
- nullable warning gate
- tests with coverage collection
- transitive dependency vulnerability scan
- upload coverage and scan artifacts

## Local Validation

```powershell
dotnet restore ELearning.sln
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

Dependency vulnerability scan, matching CI:

```powershell
dotnet list ELearning.API/ELearning.API.csproj package --vulnerable --include-transitive
dotnet list ELearning.Application/ELearning.Application.csproj package --vulnerable --include-transitive
dotnet list ELearning.Domain/ELearning.Domain.csproj package --vulnerable --include-transitive
dotnet list ELearning.Infrastructure/ELearning.Infrastructure.csproj package --vulnerable --include-transitive
dotnet list ELearning.SharedKernel/ELearning.SharedKernel.csproj package --vulnerable --include-transitive
dotnet list ELearning.Application.Tests/ELearning.Application.Tests.csproj package --vulnerable --include-transitive
dotnet list ELearning.IntegrationTests/ELearning.IntegrationTests.csproj package --vulnerable --include-transitive
```

Deployment artifact checks:

```powershell
docker compose config
kubectl kustomize deploy/kubernetes/base
```

## External Dependencies In Tests

Normal tests do not require:

- live SQL Server
- live RabbitMQ
- Docker
- Kubernetes
- SMTP

SQLite in-memory and fakes are used where practical. Docker Compose exists for local runtime validation, not as a required test dependency.
