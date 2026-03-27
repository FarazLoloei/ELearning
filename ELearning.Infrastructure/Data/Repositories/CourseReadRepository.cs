// <copyright file="CourseReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using System.Text;
using Dapper;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.ReadModels;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class CourseReadRepository(ApplicationDbContext context) : ICourseReadRepository
{
    public async Task<PaginatedList<CourseReadModel>> SearchAsync(
        string? searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("PageSize", pagination.PageSize);
        parameters.Add("Offset", pagination.SkipCount);
        parameters.Add("PublishedStatusId", CourseStatus.Published.Id);

        var whereClause = BuildWhereClause(searchTerm, categoryId, levelId, isFeatured, parameters);

        var countSql = $$"""
                         SELECT COUNT(*)
                         FROM Courses c
                         INNER JOIN Users u ON u.Id = c.InstructorId
                         {{whereClause}}
                         """;

        var itemsSql = $$"""
                         SELECT c.Id,
                                c.Title,
                                c.Description,
                                u.FirstName AS InstructorFirstName,
                                u.LastName AS InstructorLastName,
                                c.Category AS CategoryId,
                                c.Level AS LevelId,
                                c.Price,
                                COALESCE(c.AverageRatingValue, 0) AS AverageRating,
                                COALESCE(c.NumberOfRatings, 0) AS NumberOfRatings,
                                c.IsFeatured,
                                COALESCE(c.DurationHours, 0) AS DurationHours,
                                COALESCE(c.DurationMinutes, 0) AS DurationMinutes,
                                COALESCE(ec.EnrollmentsCount, 0) AS EnrollmentsCount
                         FROM Courses c
                         INNER JOIN Users u ON u.Id = c.InstructorId
                         LEFT JOIN (
                             SELECT CourseId,
                                    COUNT(DISTINCT StudentId) AS EnrollmentsCount
                             FROM Enrollments
                             GROUP BY CourseId
                         ) ec ON ec.CourseId = c.Id
                         {{whereClause}}
                         ORDER BY c.createdAtUTC DESC, c.Id
                         LIMIT @PageSize OFFSET @Offset
                         """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, parameters, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<CourseReadModel>(
            new CommandDefinition(itemsSql, parameters, cancellationToken: cancellationToken));

        return new PaginatedList<CourseReadModel>(
            [.. rows],
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private static string BuildWhereClause(
        string? searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        DynamicParameters parameters)
    {
        var filters = new List<string>();

        filters.Add("c.Status = @PublishedStatusId");

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            parameters.Add("SearchPattern", $"%{searchTerm.Trim().ToLowerInvariant()}%");
            filters.Add("(LOWER(c.Title) LIKE @SearchPattern OR LOWER(c.Description) LIKE @SearchPattern)");
        }

        if (categoryId.HasValue)
        {
            parameters.Add("CategoryId", categoryId.Value);
            filters.Add("c.Category = @CategoryId");
        }

        if (levelId.HasValue)
        {
            parameters.Add("LevelId", levelId.Value);
            filters.Add("c.Level = @LevelId");
        }

        if (isFeatured.HasValue)
        {
            parameters.Add("IsFeatured", isFeatured.Value);
            filters.Add("c.IsFeatured = @IsFeatured");
        }

        if (filters.Count == 0)
        {
            return string.Empty;
        }

        var builder = new StringBuilder();
        builder.AppendLine("WHERE");
        builder.Append("    ");
        builder.Append(string.Join(Environment.NewLine + "    AND ", filters));
        return builder.ToString();
    }

    public Task<CourseReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<CourseReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
