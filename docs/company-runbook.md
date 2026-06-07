# Company Reviewer Runbook

## Project Purpose

This repository is a backend-focused .NET Web API portfolio sample for an e-learning platform.

It is scoped as a local-reviewable backend sample, not as a full production LMS or SaaS platform.

Use this guide to clone, validate, run, and smoke-test the project with Docker Compose and Newman.

## Prerequisites

- Git
- .NET 10 SDK
- Docker Desktop with Linux containers
- Node.js LTS for `npx newman`
- Optional: Postman GUI

## Clone The Repository

```powershell
git clone https://github.com/FarazLoloei/ELearning.git
cd ELearning
```

## Baseline Validation

Run the standard solution validation commands first:

```powershell
dotnet restore ELearning.sln
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

## Optional Dependency Vulnerability Scan Matching CI

Run the same per-project dependency scan pattern used by CI:

```powershell
dotnet list ELearning.API/ELearning.API.csproj package --vulnerable --include-transitive
dotnet list ELearning.Application/ELearning.Application.csproj package --vulnerable --include-transitive
dotnet list ELearning.Domain/ELearning.Domain.csproj package --vulnerable --include-transitive
dotnet list ELearning.Infrastructure/ELearning.Infrastructure.csproj package --vulnerable --include-transitive
dotnet list ELearning.SharedKernel/ELearning.SharedKernel.csproj package --vulnerable --include-transitive
dotnet list ELearning.Application.Tests/ELearning.Application.Tests.csproj package --vulnerable --include-transitive
dotnet list ELearning.IntegrationTests/ELearning.IntegrationTests.csproj package --vulnerable --include-transitive
```

## Docker Compose Startup

Create a local environment file and validate the Compose configuration:

```powershell
Copy-Item .env.example .env
docker compose config
```

Start from a clean local container state:

```powershell
docker compose down -v --remove-orphans
docker compose up --build -d
docker compose ps
```

The first startup can take a little time while SQL Server becomes healthy and the API runs its SQL Server migrations.

## Expected Services And URLs

- Swagger UI and API root: `http://localhost:8080/`
- API base URL: `http://localhost:8080/api/v1`
- Swagger JSON: `http://localhost:8080/swagger/v1/swagger.json`
- Live health: `http://localhost:8080/health/live`
- Ready health: `http://localhost:8080/health/ready`
- RabbitMQ management UI: `http://localhost:15672`
- SQL Server: `localhost,1433` for local container review only

RabbitMQ local development credentials:

- Username: `elearning`
- Password: `elearning_dev_password`

These credentials are local development examples only. Do not reuse them for any shared or real environment.

## Health And Swagger Checks

Run the health checks:

```powershell
Invoke-RestMethod http://localhost:8080/health/live
Invoke-RestMethod http://localhost:8080/health/ready
```

Open Swagger UI:

- `http://localhost:8080/`

`/health/ready` checks database readiness. It does not confirm RabbitMQ readiness.

## Newman Smoke Test

Run the current manual smoke collection with the current environment file:

```powershell
npx newman run ".\postman\ELearning-Manual-Smoke.postman_collection.json" `
  -e ".\postman\ELearning-Manual-Smoke.postman_environment.json" `
  --env-var "baseUrl=http://localhost:8080"
```

Expected result:

- `0` failed requests
- `0` failed assertions
- optional admin-related steps may be skipped if `adminToken` is not available

The Newman collection stores created IDs automatically where required.

Without `adminToken`, the collection still validates health checks, authentication, instructor authoring, route contracts, and negative authorization checks. It does not validate the full publication, enrollment, grading, review, and certificate path because those steps depend on a published course and admin-only approval.

## Troubleshooting

- Confirm Docker Desktop is running before starting the stack.
- On Windows, confirm Docker Desktop is using Linux containers.
- Check for local port conflicts on `8080`, `1433`, `5672`, and `15672`.

View service logs:

```powershell
docker compose logs -f elearning-api
docker compose logs -f sqlserver
docker compose logs -f rabbitmq
```

If the API is not ready yet, check `docker compose ps` and wait for SQL Server and the API health checks to settle.

If RabbitMQ fails to start after a previous local run, reset the Docker Compose volumes and start again:

```powershell
docker compose down -v --remove-orphans
docker compose up --build -d
```

## Stop And Cleanup

Stop the local stack:

```powershell
docker compose down
```

Remove local container state and volumes:

```powershell
docker compose down -v --remove-orphans
```

## Final Reviewer Checklist

- `dotnet restore`, `dotnet build`, and `dotnet test` passed
- optional dependency vulnerability scan completed, if run
- `docker compose config` passed
- Compose services are healthy
- health endpoints return healthy responses
- Swagger opens at `http://localhost:8080/`
- RabbitMQ UI opens at `http://localhost:15672`
- Newman completed with `0` failed requests and `0` failed assertions

## Notes

- The full publication, enrollment, and certificate smoke path needs an admin token.
- `/health/ready` checks database readiness, not RabbitMQ readiness.
- Docker Compose is a local review setup, not a production deployment recipe.
