using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IUserRepository : IEntityFrameworkRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken = default);

    Task<int> GetUsersCountAsync(CancellationToken cancellationToken = default);
}