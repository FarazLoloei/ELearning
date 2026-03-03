using MediatR;

namespace ELearning.Infrastructure.Outbox;

public sealed record OutboxDomainEventNotification(
    Guid MessageId,
    string EventType,
    string Payload,
    DateTime OccurredOnUtc) : INotification;
