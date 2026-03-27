// <copyright file="RegisterInstructorCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Handlers;

using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.ValueObjects;
using MediatR;

public sealed class RegisterInstructorCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenIssuer accessTokenIssuer,
        IRefreshTokenStore refreshTokenStore,
        ISecurityAuditWriter securityAuditWriter)
    : IRequestHandler<RegisterInstructorCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterInstructorCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        if (!await userRepository.IsEmailUniqueAsync(email, cancellationToken))
        {
            await securityAuditWriter.WriteAsync(null, "auth.register.instructor", false, "Email already in use", cancellationToken);
            return AuthResult.Failed("Email is already in use.");
        }

        var instructor = new Instructor(
            request.FirstName,
            request.LastName,
            Email.Create(email),
            passwordHasher.HashPassword(request.Password),
            request.Bio,
            request.Expertise);

        await userRepository.AddAsync(instructor, cancellationToken);

        var accessToken = accessTokenIssuer.IssueToken(instructor);
        var refreshToken = await refreshTokenStore.IssueAsync(instructor.Id, cancellationToken);
        await securityAuditWriter.WriteAsync(instructor.Id, "auth.register.instructor", true, "Instructor registration succeeded.", cancellationToken);

        return AuthResult.Succeeded(
            accessToken,
            refreshToken,
            instructor.Id,
            instructor.Email.Value,
            instructor.FullName,
            instructor.Role.Name);
    }
}
