// <copyright file="ISecurityAuditWriter.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Abstractions;

public interface ISecurityAuditWriter
{
    Task WriteAsync(Guid? userId, string eventType, bool succeeded, string detail, CancellationToken cancellationToken = default);
}
