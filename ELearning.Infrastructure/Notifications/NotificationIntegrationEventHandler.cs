// <copyright file="NotificationIntegrationEventHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Notifications;

using System.Text.Json;
using ELearning.Application.Common.Interfaces;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class NotificationIntegrationEventHandler(
    ApplicationDbContext dbContext,
    IEmailService emailService,
    ILogger<NotificationIntegrationEventHandler> logger) : INotificationIntegrationEventHandler
{
    public const string ConsumerName = "email-notification-consumer";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<NotificationProcessingResult> HandleAsync(
        Guid messageId,
        string eventType,
        string payload,
        CancellationToken cancellationToken)
    {
        if (!IsNotificationRequested(eventType))
        {
            throw new InvalidOperationException($"Unsupported notification integration event type '{eventType}'.");
        }

        var integrationEvent = JsonSerializer.Deserialize<NotificationRequestedIntegrationEvent>(payload, JsonOptions)
            ?? throw new InvalidOperationException("Notification integration event payload could not be deserialized.");

        var effectiveMessageId = messageId == Guid.Empty ? integrationEvent.EventId : messageId;
        var alreadyProcessed = await dbContext.ProcessedIntegrationMessages
            .AnyAsync(
                x => x.MessageId == effectiveMessageId && x.Consumer == ConsumerName,
                cancellationToken);

        if (alreadyProcessed)
        {
            logger.LogInformation(
                "Skipping duplicate notification message {MessageId} for consumer {Consumer}.",
                effectiveMessageId,
                ConsumerName);
            return NotificationProcessingResult.AlreadyProcessed;
        }

        await emailService.SendEmailAsync(
            integrationEvent.RecipientEmail,
            integrationEvent.Subject,
            integrationEvent.Body,
            integrationEvent.IsHtml);

        dbContext.ProcessedIntegrationMessages.Add(new ProcessedIntegrationMessage
        {
            MessageId = effectiveMessageId,
            Consumer = ConsumerName,
            Type = nameof(NotificationRequestedIntegrationEvent),
            ProcessedOnUtc = DateTime.UtcNow,
        });

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            logger.LogInformation(
                ex,
                "Notification message {MessageId} was already recorded as processed by another consumer.",
                effectiveMessageId);

            dbContext.ChangeTracker.Clear();
            return NotificationProcessingResult.AlreadyProcessed;
        }

        return NotificationProcessingResult.Processed;
    }

    private static bool IsNotificationRequested(string eventType) =>
        eventType.Equals(nameof(NotificationRequestedIntegrationEvent), StringComparison.Ordinal) ||
        eventType.EndsWith($".{nameof(NotificationRequestedIntegrationEvent)}", StringComparison.Ordinal);

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true ||
        exception.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
        exception.InnerException?.Message.Contains("PK_", StringComparison.OrdinalIgnoreCase) == true;
}
