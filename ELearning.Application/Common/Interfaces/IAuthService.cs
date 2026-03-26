// <copyright file="IAuthService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Interfaces;

using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise, CancellationToken cancellationToken = default);

    Task<AuthResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<Result> RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<string> GenerateJwtToken(User user);
}
