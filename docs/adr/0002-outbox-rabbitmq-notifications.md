# ADR 0002: Outbox, RabbitMQ, And Notifications

## Status

Accepted

## Context

The application sends lightweight notifications for important outcomes such as enrollment, course approval, grading, and certificate issuance.

Direct email calls are simple, but they couple user-facing workflows to notification delivery. A public backend sample also benefits from demonstrating reliable asynchronous integration without becoming a microservice system.

## Decision

Use the outbox pattern and RabbitMQ for integration-event publishing, while keeping the application a modular monolith.

Notification delivery uses:

- outbox messages persisted with application changes
- RabbitMQ topic exchange for integration events
- `notification.requested.v1` event for email-ready notification payloads
- idempotent consumer tracking through `ProcessedIntegrationMessages`
- existing `IEmailService` seam for delivery behavior

RabbitMQ is config-gated and disabled by default so local tests do not require a broker.

## Consequences

Benefits:

- application workflows do not depend on immediate broker availability when writing state
- notification delivery can be asynchronous when RabbitMQ is enabled
- duplicate consumer messages are handled idempotently
- the sample demonstrates practical reliability patterns

Tradeoffs:

- there is more infrastructure code than direct email calls
- notification history is not a full product feature
- no delayed retry queue or operational replay UI exists yet

## Alternatives Considered

- Direct email only: simpler, but less useful as a reliability sample.
- MassTransit: powerful, but heavier than needed for this focused portfolio project.
- Separate worker service: postponed until independent deployment or scaling is actually needed.
