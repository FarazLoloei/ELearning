// <copyright file="OutboxMessage.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Models;

public sealed class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime OccurredOnUtc { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public DateTime? ProcessedOnUtc { get; set; }

    public int RetryCount { get; set; }

    public string? Error { get; set; }
}
