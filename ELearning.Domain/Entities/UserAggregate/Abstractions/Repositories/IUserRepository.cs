// <copyright file="IUserRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

public interface IUserRepository : IEntityFrameworkRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    Task<int> GetUsersCountAsync(CancellationToken cancellationToken = default);
}