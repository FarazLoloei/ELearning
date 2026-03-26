// <copyright file="ProgressReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using Dapper;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Enrollments.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class ProgressReadRepository(ApplicationDbContext context) : IProgressReadRepository
{
    public async Task<ProgressReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT
                               Id,
                               EnrollmentId,
                               LessonId,
                               Status AS StatusId,
                               CASE Status
                                   WHEN 1 THEN 'NotStarted'
                                   WHEN 2 THEN 'InProgress'
                                   WHEN 3 THEN 'Completed'
                                   ELSE 'Unknown'
                               END AS StatusName,
                               CompletedDate,
                               TimeSpentSeconds
                           FROM Progresses
                           WHERE Id = @Id
                           """;

        return await connection.QuerySingleOrDefaultAsync<ProgressReadModel>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<ProgressReadModel>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT
                               Id,
                               EnrollmentId,
                               LessonId,
                               Status AS StatusId,
                               CASE Status
                                   WHEN 1 THEN 'NotStarted'
                                   WHEN 2 THEN 'InProgress'
                                   WHEN 3 THEN 'Completed'
                                   ELSE 'Unknown'
                               END AS StatusName,
                               CompletedDate,
                               TimeSpentSeconds
                           FROM Progresses
                           WHERE EnrollmentId = @EnrollmentId
                           ORDER BY Id
                           """;

        var rows = await connection.QueryAsync<ProgressReadModel>(
            new CommandDefinition(sql, new { EnrollmentId = enrollmentId }, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<ProgressReadModel?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT
                               Id,
                               EnrollmentId,
                               LessonId,
                               Status AS StatusId,
                               CASE Status
                                   WHEN 1 THEN 'NotStarted'
                                   WHEN 2 THEN 'InProgress'
                                   WHEN 3 THEN 'Completed'
                                   ELSE 'Unknown'
                               END AS StatusName,
                               CompletedDate,
                               TimeSpentSeconds
                           FROM Progresses
                           WHERE EnrollmentId = @EnrollmentId AND LessonId = @LessonId
                           """;

        return await connection.QuerySingleOrDefaultAsync<ProgressReadModel>(
            new CommandDefinition(sql, new { EnrollmentId = enrollmentId, LessonId = lessonId }, cancellationToken: cancellationToken));
    }

    public async Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT
                               CASE
                                   WHEN stats.TotalLessons = 0 THEN 0.0
                                   ELSE (CAST(stats.CompletedLessons AS FLOAT) / CAST(stats.TotalLessons AS FLOAT)) * 100.0
                               END
                           FROM (
                               SELECT
                                   (
                                       SELECT COUNT(1)
                                       FROM Lessons l
                                       INNER JOIN Modules m ON m.Id = l.ModuleId
                                       INNER JOIN Enrollments e ON e.CourseId = m.CourseId
                                       WHERE e.Id = @EnrollmentId
                                   ) AS TotalLessons,
                                   (
                                       SELECT COUNT(1)
                                       FROM Progresses
                                       WHERE EnrollmentId = @EnrollmentId AND Status = 3
                                   ) AS CompletedLessons
                           ) stats
                           """;

        var percentage = await connection.QuerySingleOrDefaultAsync<double?>(
            new CommandDefinition(sql, new { EnrollmentId = enrollmentId }, cancellationToken: cancellationToken));

        return percentage ?? 0;
    }

    public async Task<PaginatedList<ProgressReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Progresses
                                """;

        const string sql = """
                           SELECT
                               Id,
                               EnrollmentId,
                               LessonId,
                               Status AS StatusId,
                               CASE Status
                                   WHEN 1 THEN 'NotStarted'
                                   WHEN 2 THEN 'InProgress'
                                   WHEN 3 THEN 'Completed'
                                   ELSE 'Unknown'
                               END AS StatusName,
                               CompletedDate,
                               TimeSpentSeconds
                           FROM Progresses
                           ORDER BY EnrollmentId, LessonId, Id
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<ProgressReadModel>(
            new CommandDefinition(
                sql,
                new
                {
                    pagination.PageSize,
                    Offset = pagination.SkipCount,
                },
                cancellationToken: cancellationToken));

        return new PaginatedList<ProgressReadModel>(
            [.. rows],
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }
}