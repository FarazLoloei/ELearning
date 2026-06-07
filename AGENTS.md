# AGENTS.md

Project: E-Learning Backend Portfolio Project
Purpose: Lightweight guidance for AI coding agents working in this repository.

## Project Identity

This repository is a backend-focused .NET e-learning Web API portfolio project.

It is not:
- SaaS
- full-stack
- Labora
- an offline-first system
- a multi-tenant enterprise platform

Keep changes aligned with a professional backend portfolio sample.

## Solution Map

Main projects:

- `ELearning.API` — ASP.NET Core entry point, REST controllers, GraphQL, middleware, health, auth wiring
- `ELearning.Application` — commands, queries, handlers, validators, application behaviors
- `ELearning.Domain` — domain entities, aggregates, domain rules, domain events
- `ELearning.Infrastructure` — EF Core, Dapper read models, repositories, auth/token persistence, messaging/outbox/RabbitMQ
- `ELearning.SharedKernel` — shared abstractions and cross-cutting primitives
- `ELearning.Application.Tests` — domain/application/architecture tests
- `ELearning.IntegrationTests` — API and infrastructure-oriented integration tests

## Working Rules

- Inspect existing code before planning changes.
- Make the smallest safe change.
- Do not rewrite unrelated code.
- Do not introduce frontend work.
- Do not introduce SaaS/multi-tenancy/offline-first patterns unless explicitly requested.
- Keep README claims aligned with actual implementation evidence.
- Do not strengthen portfolio claims without code/tests/docs support.
- Do not commit, push, create PRs, or merge unless explicitly requested.

## Validation Commands

Preferred validation commands:

```powershell
dotnet restore ELearning.sln
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet list ELearning.sln package --vulnerable --include-transitive
docker compose config
kubectl kustomize deploy/kubernetes/base
```

Use targeted tests when possible for small changes.

## Provider Defaults

The project supports SQLite in-memory for local/test defaults and SQL Server for migration-backed runtime scenarios.

Do not change provider behavior casually. If a change affects provider selection, migrations, or test database behavior, document it clearly and run relevant tests.

## Messaging

RabbitMQ messaging is optional/config-gated. Do not assume RabbitMQ is always enabled.

Outbox and notification processing are part of the project story. Changes around messaging should preserve idempotency and existing integration-event behavior.

## Secrets And Configuration

Development/example values may exist for local Docker/RabbitMQ/SQL Server usage. Do not add real secrets.

If editing docs or config examples, clearly mark example credentials as local/dev-only.

## Documentation Rules

Update README/docs when behavior, validation commands, test counts, deployment behavior, or public claims change.

Avoid stale hardcoded test counts when possible. If counts are kept, make sure they match the latest test run.

## Portfolio Rules

This repository is reviewed by employers. Prefer clarity, correctness, and honesty.

Good portfolio changes:
- improve README accuracy
- improve test reliability
- clarify architecture
- remove confusing leftovers
- tighten security notes
- keep CI green

Bad portfolio changes:
- over-engineering
- adding unused abstractions
- adding frontend scope
- claiming production readiness without evidence
- hiding limitations
