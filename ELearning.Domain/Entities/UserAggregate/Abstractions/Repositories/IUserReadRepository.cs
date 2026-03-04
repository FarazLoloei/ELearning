using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IUserReadRepository
{
    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);
}