// <copyright file="IIntegrationEventPublisher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IntegrationEventPublishMessage message, CancellationToken cancellationToken);
}
