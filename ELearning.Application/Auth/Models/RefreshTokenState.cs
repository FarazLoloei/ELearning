// <copyright file="RefreshTokenState.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Models;

public sealed record RefreshTokenState(
    Guid UserId,
    DateTime ExpiresAtUtc,
    DateTime? RevokedAtUtc)
{
    public bool IsRevoked => this.RevokedAtUtc.HasValue;

    public bool IsExpired(DateTime utcNow) => this.ExpiresAtUtc <= utcNow;
}
