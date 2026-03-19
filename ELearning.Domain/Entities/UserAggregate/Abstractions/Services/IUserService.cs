// <copyright file="IUserService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Services;

public interface IUserService
{
    Task<bool> IsEmailUniqueAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);

    string HashPassword(string password);

    Task<bool> VerifyPasswordAsync(string hashedPassword, string providedPassword);
}
