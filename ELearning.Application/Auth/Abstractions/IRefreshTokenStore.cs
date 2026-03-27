// <copyright file="IRefreshTokenStore.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Abstractions;

using ELearning.Application.Auth.Models;

public interface IRefreshTokenStore
{
    Task<RefreshTokenState?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<string> RotateAsync(string refreshToken, string revokedReason, CancellationToken cancellationToken = default);

    Task RevokeAsync(string refreshToken, string reason, CancellationToken cancellationToken = default);
}
