// <copyright file="SecurityAuditEvent.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Models;

public sealed class SecurityAuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? UserId { get; set; }

    public string EventType { get; set; } = string.Empty;

    public bool Succeeded { get; set; }

    public string? Detail { get; set; }

    public string? IpAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime OccurredOnUtc { get; set; } = DateTime.UtcNow;
}
