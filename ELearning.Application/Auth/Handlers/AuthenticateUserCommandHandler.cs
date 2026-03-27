// <copyright file="AuthenticateUserCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Handlers;

using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public sealed class AuthenticateUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenIssuer accessTokenIssuer,
        IRefreshTokenStore refreshTokenStore,
        ISecurityAuditWriter securityAuditWriter)
    : IRequestHandler<AuthenticateUserCommand, AuthResult>
{
    public async Task<AuthResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            await securityAuditWriter.WriteAsync(null, "auth.login", false, "User not found", cancellationToken);
            return AuthResult.Failed("Authentication failed.");
        }

        if (!passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            await securityAuditWriter.WriteAsync(user.Id, "auth.login", false, "Invalid password", cancellationToken);
            return AuthResult.Failed("Authentication failed.");
        }

        if (!user.IsActive)
        {
            await securityAuditWriter.WriteAsync(user.Id, "auth.login", false, "User inactive", cancellationToken);
            return AuthResult.Failed("Authentication failed.");
        }

        user.RecordLogin();
        await userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = accessTokenIssuer.IssueToken(user);
        var refreshToken = await refreshTokenStore.IssueAsync(user.Id, cancellationToken);
        await securityAuditWriter.WriteAsync(user.Id, "auth.login", true, "User login succeeded.", cancellationToken);

        return AuthResult.Succeeded(
            accessToken,
            refreshToken,
            user.Id,
            user.Email.Value,
            user.FullName,
            user.Role.Name);
    }
}
