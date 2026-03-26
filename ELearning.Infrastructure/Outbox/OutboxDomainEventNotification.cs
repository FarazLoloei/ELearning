// <copyright file="OutboxDomainEventNotification.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using MediatR;

public sealed record OutboxDomainEventNotification(
    Guid MessageId,
    string EventType,
    string Payload,
    DateTime OccurredOnUtc) : INotification;
