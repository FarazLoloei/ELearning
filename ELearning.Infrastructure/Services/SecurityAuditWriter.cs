// <copyright file="SecurityAuditWriter.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Services;

using ELearning.Application.Auth.Abstractions;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;

public sealed class SecurityAuditWriter(
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor) : ISecurityAuditWriter
{
    public Task WriteAsync(Guid? userId, string eventType, bool succeeded, string detail, CancellationToken cancellationToken = default)
    {
        dbContext.SecurityAuditEvents.Add(new SecurityAuditEvent
        {
            UserId = userId,
            EventType = eventType,
            Succeeded = succeeded,
            Detail = detail,
            IpAddress = this.GetRemoteIpAddress(),
            UserAgent = this.GetUserAgent(),
            OccurredOnUtc = DateTime.UtcNow,
        });

        return Task.CompletedTask;
    }

    private string? GetRemoteIpAddress() => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString();
}
