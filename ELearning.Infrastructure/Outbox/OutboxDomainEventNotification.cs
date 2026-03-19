// <copyright file="OutboxDomainEventNotification.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using MediatR;

public sealed record OutboxDomainEventNotification(
    Guid MessageId,
    string EventType,
    string Payload,
    DateTime OccurredOnUtc) : INotification;
