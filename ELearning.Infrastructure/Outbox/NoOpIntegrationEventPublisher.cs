// <copyright file="NoOpIntegrationEventPublisher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

public sealed class NoOpIntegrationEventPublisher : IIntegrationEventPublisher
{
    public Task PublishAsync(IntegrationEventPublishMessage message, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
