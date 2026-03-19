// <copyright file="UserReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System;
using System.Collections.Generic;
using System.Text;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class UserReadRepository(ApplicationDbContext context) : IUserReadRepository
{
    /// <summary>
    /// Retrieves a read-only list of users by their role.
    /// </summary>
    /// <param name="role">The role of the users to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of users matching the specified role.</returns>
    public async Task<IReadOnlyList<User>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken) =>
        await context.Users
            .AsNoTracking()
            .Where(u => u.Role == role)
            .ToListAsync(cancellationToken);

    /// <summary>
    /// Searches for users based on a search term with pagination.
    /// </summary>
    /// <param name="searchTerm">The term to search for in first name, last name, or email.</param>
    /// <param name="pagination">Pagination parameters (SkipCount, PageSize).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A read-only list of users matching the search criteria and pagination.</returns>
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
}