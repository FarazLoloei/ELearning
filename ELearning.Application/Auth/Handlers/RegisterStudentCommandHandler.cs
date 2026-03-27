// <copyright file="RegisterStudentCommandHandler.cs" company="FarazLoloei">
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

public sealed class RegisterStudentCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IAccessTokenIssuer accessTokenIssuer,
        IRefreshTokenStore refreshTokenStore,
        ISecurityAuditWriter securityAuditWriter)
    : IRequestHandler<RegisterStudentCommand, AuthResult>
{
    public async Task<AuthResult> Handle(RegisterStudentCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        if (!await userRepository.IsEmailUniqueAsync(email, cancellationToken))
        {
            await securityAuditWriter.WriteAsync(null, "auth.register.student", false, "Email already in use", cancellationToken);
            return AuthResult.Failed("Email is already in use.");
        }

        var student = new Student(
            request.FirstName,
            request.LastName,
            Email.Create(email),
            passwordHasher.HashPassword(request.Password));

        await userRepository.AddAsync(student, cancellationToken);

        var accessToken = accessTokenIssuer.IssueToken(student);
        var refreshToken = await refreshTokenStore.IssueAsync(student.Id, cancellationToken);
        await securityAuditWriter.WriteAsync(student.Id, "auth.register.student", true, "Student registration succeeded.", cancellationToken);

        return AuthResult.Succeeded(
            accessToken,
            refreshToken,
            student.Id,
            student.Email.Value,
            student.FullName,
            student.Role.Name);
    }
}
