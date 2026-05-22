# ADR 0001: Modular Monolith

## Status

Accepted

## Context

The project is a public backend portfolio sample for an e-learning domain. It needs to demonstrate senior engineering judgment without becoming difficult to review.

The domain has multiple capabilities:

- auth
- course authoring
- enrollment
- progression
- assessments
- certificates
- notifications

These capabilities are related enough that splitting them into services would add distributed-system complexity before there is a real scaling or team-boundary need.

## Decision

Use a modular monolith with Clean Architecture-style dependency direction.

The codebase keeps separate projects for API, Application, Domain, Infrastructure, SharedKernel, and tests. Modules are separated by folders and use cases rather than process boundaries.

## Consequences

Benefits:

- easier to run and review
- transactions remain straightforward
- domain workflows stay cohesive
- architecture still demonstrates clear boundaries
- later extraction remains possible if a real need appears

Tradeoffs:

- modules are not independently deployable
- scaling is at the application level
- boundaries rely on code structure, tests, and discipline rather than network isolation

## Alternatives Considered

- Microservices: rejected because they would add operational overhead and messaging complexity not justified by the sample.
- Simple CRUD layering: rejected because it would not demonstrate the product workflows and domain behavior the project is meant to show.
