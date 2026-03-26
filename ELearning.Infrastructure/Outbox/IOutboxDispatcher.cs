// <copyright file="IOutboxDispatcher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

public interface IOutboxDispatcher
{
    Task<int> DispatchPendingAsync(CancellationToken cancellationToken);
}
