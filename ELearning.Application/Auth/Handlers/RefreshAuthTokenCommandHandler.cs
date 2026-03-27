// <copyright file="RefreshAuthTokenCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Handlers;

using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public sealed class RefreshAuthTokenCommandHandler(
        IRefreshTokenStore refreshTokenStore,
        IUserRepository userRepository,
        IAccessTokenIssuer accessTokenIssuer,
        ISecurityAuditWriter securityAuditWriter)
    : IRequestHandler<RefreshAuthTokenCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RefreshAuthTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshTokenState = await refreshTokenStore.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshTokenState is null || refreshTokenState.IsRevoked || refreshTokenState.IsExpired(DateTime.UtcNow))
        {
            await securityAuditWriter.WriteAsync(
                refreshTokenState?.UserId,
                "auth.refresh",
                false,
                "Refresh token invalid or expired",
                cancellationToken);

            return AuthResult.Failed("Refresh token is invalid.");
        }

        var user = await userRepository.GetByIdForUpdateAsync(refreshTokenState.UserId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            await securityAuditWriter.WriteAsync(
                refreshTokenState.UserId,
                "auth.refresh",
                false,
                "User not found or inactive",
                cancellationToken);

            return AuthResult.Failed("Refresh token is invalid.");
        }

        var newRefreshToken = await refreshTokenStore.RotateAsync(request.RefreshToken, "Rotated", cancellationToken);
        var accessToken = accessTokenIssuer.IssueToken(user);
        await securityAuditWriter.WriteAsync(user.Id, "auth.refresh", true, "Refresh token rotated.", cancellationToken);

        return AuthResult.Succeeded(
            accessToken,
            newRefreshToken,
            user.Id,
            user.Email.Value,
            user.FullName,
            user.Role.Name);
    }
}
