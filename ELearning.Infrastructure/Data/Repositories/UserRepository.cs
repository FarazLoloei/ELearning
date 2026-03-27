// <copyright file="UserRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The GUID of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The User if found, otherwise null.</returns>
    public async Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Users.FindAsync(id, cancellationToken);

    /// <summary>
    /// Adds a new user to the database.
    /// </summary>
    /// <param name="entity">The user entity to add.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public async Task AddAsync(User entity, CancellationToken cancellationToken)
    {
        await context.Users.AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Updates an existing user in the database.
    /// </summary>
    /// <param name="entity">The user entity to update.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public Task UpdateAsync(User entity, CancellationToken cancellationToken)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Deletes a user from the database.
    /// </summary>
    /// <param name="entity">The user entity to delete.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    public Task DeleteAsync(User entity, CancellationToken cancellationToken)
    {
        context.Users.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The User if found, otherwise null.</returns>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Email.Value == email, cancellationToken);

    /// <summary>
    /// Checks if an email address is unique in the database.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>True if the email is unique, false otherwise.</returns>
    public async Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken) =>
        !await context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Email.Value == email, cancellationToken);

    /// <summary>
    /// Gets the total count of users in the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The total number of users.</returns>
    public async Task<int> GetUsersCountAsync(CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
}
