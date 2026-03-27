// <copyright file="RevokeRefreshTokenCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Handlers;

using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using MediatR;

public sealed class RevokeRefreshTokenCommandHandler(
        IRefreshTokenStore refreshTokenStore,
        ISecurityAuditWriter securityAuditWriter,
        ICurrentUserService currentUserService)
    : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var refreshTokenState = await refreshTokenStore.GetByTokenAsync(request.RefreshToken, cancellationToken);
        if (refreshTokenState is null)
        {
            await securityAuditWriter.WriteAsync(null, "auth.revoke", false, "Refresh token not found", cancellationToken);
            return Result.Failure("Refresh token was not found.");
        }

        var currentUserId = currentUserService.UserId.Value;
        if (refreshTokenState.UserId != currentUserId && !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        if (!refreshTokenState.IsRevoked)
        {
            await refreshTokenStore.RevokeAsync(request.RefreshToken, "Revoked by request", cancellationToken);
        }

        await securityAuditWriter.WriteAsync(refreshTokenState.UserId, "auth.revoke", true, "Refresh token revoked.", cancellationToken);
        return Result.Success();
    }
}
