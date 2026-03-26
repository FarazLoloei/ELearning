// <copyright file="IUserReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

using System;
using System.Collections.Generic;
using System.Text;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;

public interface IUserReadRepository
{
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
}