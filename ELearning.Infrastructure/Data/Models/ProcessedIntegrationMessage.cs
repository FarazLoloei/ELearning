// <copyright file="ProcessedIntegrationMessage.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Models;

public sealed class ProcessedIntegrationMessage
{
    public Guid MessageId { get; set; }

    public string Consumer { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public DateTime ProcessedOnUtc { get; set; }

    public string? Error { get; set; }
}
