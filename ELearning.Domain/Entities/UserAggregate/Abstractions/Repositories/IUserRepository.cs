using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByEmailAsync(string email);

    Task<bool> IsEmailUniqueAsync(string email);

    Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role);

    Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize);

    Task<int> GetUsersCountAsync();
}