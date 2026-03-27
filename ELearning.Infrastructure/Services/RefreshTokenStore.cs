// <copyright file="RefreshTokenStore.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Services;

using System.Security.Cryptography;
using System.Text;
using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Auth.Models;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public sealed class RefreshTokenStore(
        ApplicationDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration) : IRefreshTokenStore
{
    private const int RefreshTokenByteSize = 64;

    public async Task<RefreshTokenState?> GetByTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var token = await dbContext.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        return token is null
            ? null
            : new RefreshTokenState(token.UserId, token.ExpiresAtUtc, token.RevokedAtUtc);
    }

    public Task<string> IssueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rawRefreshToken = GenerateRefreshToken();
        var tokenHash = HashToken(rawRefreshToken);

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(this.GetRefreshTokenExpiryDays()),
            CreatedByIp = this.GetRemoteIpAddress(),
            UserAgent = this.GetUserAgent(),
        });

        return Task.FromResult(rawRefreshToken);
    }

    public async Task<string> RotateAsync(string refreshToken, string revokedReason, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var currentToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken)
            ?? throw new InvalidOperationException("Refresh token was not found.");

        var newRefreshToken = GenerateRefreshToken();
        var newRefreshTokenHash = HashToken(newRefreshToken);

        currentToken.RevokedAtUtc ??= DateTime.UtcNow;
        currentToken.RevokedReason = revokedReason;
        currentToken.ReplacedByTokenHash = newRefreshTokenHash;

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = currentToken.UserId,
            TokenHash = newRefreshTokenHash,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(this.GetRefreshTokenExpiryDays()),
            CreatedByIp = this.GetRemoteIpAddress(),
            UserAgent = this.GetUserAgent(),
        });

        return newRefreshToken;
    }

    public async Task RevokeAsync(string refreshToken, string reason, CancellationToken cancellationToken = default)
    {
        var tokenHash = HashToken(refreshToken);
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (token is null || token.RevokedAtUtc.HasValue)
        {
            return;
        }

        token.RevokedAtUtc = DateTime.UtcNow;
        token.RevokedReason = reason;
    }

    private int GetRefreshTokenExpiryDays()
    {
        var value = configuration["JwtSettings:RefreshTokenExpiryInDays"];
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 14;
    }

    private static string GenerateRefreshToken()
    {
        Span<byte> bytes = stackalloc byte[RefreshTokenByteSize];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string HashToken(string token)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(hash);
    }

    private string? GetRemoteIpAddress() => httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

    private string? GetUserAgent() => httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString();
}
