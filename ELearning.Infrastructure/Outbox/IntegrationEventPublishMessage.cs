// <copyright file="IntegrationEventPublishMessage.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

public sealed record IntegrationEventPublishMessage(
    Guid MessageId,
    string EventType,
    string RoutingKey,
    string Payload,
    DateTime OccurredOnUtc);
