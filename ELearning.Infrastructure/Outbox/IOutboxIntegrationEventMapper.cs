// <copyright file="IOutboxIntegrationEventMapper.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using ELearning.Infrastructure.Data.Models;

public interface IOutboxIntegrationEventMapper
{
    IntegrationEventPublishMessage? Map(OutboxMessage message);
}
