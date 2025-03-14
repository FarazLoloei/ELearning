using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IUserRepository : IEntityFrameworkRepository<User>
{
    Task<User> GetByEmailAsync(string email);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role);

    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize);

    Task<int> GetUsersCountAsync();
}