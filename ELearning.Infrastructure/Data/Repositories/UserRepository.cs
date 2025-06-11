using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class UserRepository(ApplicationDbContext _context) : IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The GUID of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The User if found, otherwise null.</returns>
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await _context.Users.FindAsync(id, cancellationToken);

    /// <summary>
    /// Retrieves a read-only list of all users.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of users.</returns>
    public async Task<IReadOnlyList<User>> ListAllAsync(CancellationToken cancellationToken) =>
        await _context.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="entity">The user entity to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public async Task AddAsync(User entity, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="entity">The user entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public async Task UpdateAsync(User entity, CancellationToken cancellationToken)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="entity">The user entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public async Task DeleteAsync(User entity, CancellationToken cancellationToken)
    {
        _context.Users.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The User if found, otherwise null.</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

    /// <summary>
    /// Checks if an email address is unique in the database.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>True if the email is unique, false otherwise.</returns>
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken) =>
        !await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.Value == email);

    /// <summary>
    /// Retrieves a read-only list of users by their role.
    /// </summary>
    /// <param name="role">The role of the users to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of users matching the specified role.</returns>
    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken) =>
        await _context.Users
            .AsNoTracking()
            .Where(u => u.Role == role)
            .ToListAsync();

    /// <summary>
    /// Searches for users based on a search term with pagination.
    /// </summary>
    /// <param name="searchTerm">The term to search for in first name, last name, or email.</param>
    /// <param name="pagination">Pagination parameters (SkipCount, PageSize).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of users matching the search criteria and pagination.</returns>
    public async Task<IReadOnlyList<User>> SearchUsersAsync(string searchTerm, PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = _context.Users
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

    /// <summary>
    /// Gets the total count of users in the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The total number of users.</returns>
    public async Task<int> GetUsersCountAsync(CancellationToken cancellationToken) =>
        await _context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
}