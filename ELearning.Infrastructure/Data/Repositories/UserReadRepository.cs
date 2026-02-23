using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class UserReadRepository(ApplicationDbContext context)
{
    public async Task<IReadOnlyList<User>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = context.Users
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(u =>
                u.FirstName.ToLower().Contains(lowerCaseSearchTerm) ||
                u.LastName.ToLower().Contains(lowerCaseSearchTerm) ||
                u.Email.Value.ToLower().Contains(lowerCaseSearchTerm));
        }

        return await query
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .Skip(pagination.SkipCount)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUsersCountAsync(CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
}