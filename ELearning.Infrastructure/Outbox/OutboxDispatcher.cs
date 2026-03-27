// <copyright file="OutboxDispatcher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using ELearning.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class OutboxDispatcher(
    ApplicationDbContext dbContext,
    IPublisher publisher,
    ILogger<OutboxDispatcher> logger) : IOutboxDispatcher
{
    private const int BatchSize = 50;

    public async Task<int> DispatchPendingAsync(CancellationToken cancellationToken)
    {
        var pendingMessages = await dbContext.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (pendingMessages.Count == 0)
        {
            return 0;
        }

        foreach (var message in pendingMessages)
        {
            try
            {
                await publisher.Publish(
                    new OutboxDomainEventNotification(
                        message.Id,
                        message.Type,
                        message.Payload,
                        message.OccurredOnUtc),
                    cancellationToken);

                message.ProcessedOnUtc = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message.Length > 4096 ? ex.Message[..4096] : ex.Message;
                logger.LogError(ex, "Failed to dispatch outbox message {OutboxMessageId}", message.Id);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return pendingMessages.Count;
    }
}
