# E-Learning Platform API

Production-inspired backend sample for an e-learning platform built as a modular monolith with Clean Architecture and DDD-inspired design.

This repository exists as a senior-level backend portfolio project. The goal is not to showcase every possible feature, but to show deliberate architecture, explicit business workflows, and credible product behavior in a codebase that is still practical to review.

## Why This Project Exists

Many sample backends stop at CRUD. This project is intentionally shaped around product workflows instead:

- instructors author courses and submit them for review
- admins govern publication through explicit moderation actions
- students enroll in published courses
- learners progress through lessons and assessments
- course completion is earned through real eligibility rules
- completed learners can review courses and receive certificates

The result is a sample that is closer to a real product backend than a database wrapper.

## Product Overview

The platform models three primary actors:

- `Student`
- `Instructor`
- `Admin`

Core business capabilities currently implemented:

- identity and access with JWT auth and refresh-token rotation
- course authoring and explicit course lifecycle governance
- enrollment with lifecycle-aware eligibility
- lesson progression and rule-driven course completion
- assessment submission and grading
- reviews and ratings
- certificate issuance and verification
- lightweight notifications for important outcomes

## Architecture Overview

The solution is a modular monolith with clear inward dependency direction:

- `ELearning.API`
  REST controllers, GraphQL schema, transport concerns, composition root
- `ELearning.Application`
  commands, queries, handlers, DTOs, orchestration, validation, transaction pipeline
- `ELearning.Domain`
  aggregates, entities, value objects, invariants, explicit business behavior
- `ELearning.Infrastructure`
  EF Core, Dapper read models, repositories, auth adapters, email delivery, outbox dispatcher
- `ELearning.SharedKernel`
  small shared abstractions and base types
- `ELearning.Application.Tests`
  domain/application-focused tests
- `ELearning.IntegrationTests`
  end-to-end HTTP and authorization coverage

High-level dependency direction:

- `API -> Application`
- `Infrastructure -> Application + Domain`
- `Application -> Domain + SharedKernel`
- `Domain -> SharedKernel`

## Domain And Workflow Highlights

This project is intentionally centered on explicit workflows:

- `Course` owns lifecycle transitions such as draft creation, review submission, approval, rejection, and archiving
- `Enrollment` owns progression, assessment submission, review eligibility, and completion state
- assessment definitions stay with course authoring, while assessment execution is handled through enrollment-centered workflow
- certificates are issued only for legitimately completed enrollments and only once per enrollment

Examples of non-CRUD behavior in the current model:

- students can enroll only in published courses
- only draft/rejected courses are instructor-editable
- course completion requires lessons and required assessments
- a course review is allowed only after authoritative completion
- a certificate can be verified by public certificate code

## Example Workflows

### Course Governance

1. Instructor creates a course in `Draft`
2. Instructor submits the course for review
3. Admin approves publication or rejects with feedback
4. Published courses become visible in the public catalog

### Learning And Completion

1. Student enrolls in a published course
2. Student starts and completes lessons explicitly
3. Student submits required assessments
4. Completion is earned when authoritative eligibility is satisfied
5. Certificate issuance becomes available for that completed enrollment

### Post-Completion Outcomes

1. Student submits a review for the completed course
2. Course rating is updated through the domain flow
3. Student can retrieve or verify the issued certificate

## API Surface Overview

REST is the primary interface. GraphQL is available as a secondary interface that reuses the same application use cases.

Representative REST capabilities:

- auth: login, register, refresh, revoke
- courses: catalog queries, lifecycle actions, moderation actions, reviews
- enrollments: enroll, progression actions, review submission
- submissions: submit assessment, grade assessment
- certificates: issue, retrieve by enrollment, verify by public code

Transport design notes:

- REST responses use a consistent response envelope
- GraphQL is intentionally secondary and does not duplicate business logic
- controllers and GraphQL mutations stay thin and delegate to application handlers

## Key Technical Decisions And Tradeoffs

- The project uses a modular monolith instead of microservices to keep the sample cohesive and reviewable.
- Domain modeling is DDD-inspired, not dogmatic. Aggregates are used where they add clarity and invariant protection.
- The application layer orchestrates use cases explicitly instead of relying on broad services or fat controllers.
- Infrastructure contains adapters and persistence details, not business workflow orchestration.
- Notifications are intentionally lightweight. The project uses the existing email seam for valuable outcomes instead of building a full notification center.
- Certificates are simple, verifiable records. This sample does not add document rendering or PDF generation because that would add more infrastructure spectacle than product signal.

## Running The Project

Prerequisites:

- .NET 10 SDK

Configure local secrets for JWT settings:

```powershell
dotnet user-secrets set "JwtSettings:Issuer" "elearning-local" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:Audience" "elearning-local" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:Secret" "your-long-random-secret-at-least-32-characters" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:ExpiryInDays" "7" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "JwtSettings:RefreshTokenExpiryInDays" "14" --project ELearning.API/ELearning.API.csproj
dotnet user-secrets set "Ocelot:Enabled" "false" --project ELearning.API/ELearning.API.csproj
```

Run the API:

```powershell
dotnet run --project ELearning.API/ELearning.API.csproj
```

Useful endpoints:

- Swagger UI: `/`
- REST: `/api/v1/*` and compatibility routes under `/api/*`
- GraphQL: `/graphql`

Database notes:

- sqlite in-memory is the default local experience
- SQL Server can be configured through the existing provider settings

## Build And Test

```powershell
dotnet build ELearning.sln -nologo /p:UseSharedCompilation=false
dotnet test ELearning.sln -nologo /p:UseSharedCompilation=false
```

Current local verification:

- `ELearning.Application.Tests`: `106` passing
- `ELearning.IntegrationTests`: `22` passing

## Why This Is A Strong Backend Sample

This project is strongest where reviewers usually look for senior-level judgment:

- explicit workflow modeling instead of generic status mutation
- deliberate layer boundaries with architecture tests
- domain rules placed in aggregates and application use cases instead of controllers
- pragmatic read/write separation without unnecessary distributed complexity
- credible auth, moderation, progression, assessment, review, certificate, and notification flows

It is intentionally not a “perfect enterprise platform.” It is a realistic sample that balances architecture quality with repo readability.

## Future Improvements

Good next steps if the project were extended further:

- richer authoring flows for modules, lessons, and assessments
- review moderation and review editing policies
- certificate rendering/export if the product needs it
- stronger notification persistence/history beyond email delivery
- deployment guidance and production environment configuration examples

## License

MIT. See [LICENSE.txt](LICENSE.txt).
