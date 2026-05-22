// <copyright file="INotificationIntegrationEventHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Notifications;

public interface INotificationIntegrationEventHandler
{
    Task<NotificationProcessingResult> HandleAsync(
        Guid messageId,
        string eventType,
        string payload,
        CancellationToken cancellationToken);
}
