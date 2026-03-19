// <copyright file="StudentReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using Dapper;
using ELearning.Application.Students.Abstractions;
using ELearning.Application.Students.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class StudentReadRepository(ApplicationDbContext context) : IStudentReadRepository
{
    public async Task<StudentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT Id, FirstName, LastName, Email, ProfilePictureUrl, LastLoginDate
                           FROM Users
                           WHERE Id = @Id AND UserType = 'Student'
                           """;

        return await connection.QuerySingleOrDefaultAsync<StudentReadModel>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<StudentReadModel>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT DISTINCT u.Id, u.FirstName, u.LastName, u.Email, u.ProfilePictureUrl, u.LastLoginDate
                           FROM Users u
                           INNER JOIN Enrollments e ON e.StudentId = u.Id
                           WHERE u.UserType = 'Student' AND e.CourseId = @CourseId
                           ORDER BY u.LastName, u.FirstName
                           """;

        var rows = await connection.QueryAsync<StudentReadModel>(
            new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<IReadOnlyList<StudentCourseReadModel>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT c.Id, c.Title, c.Category AS CategoryId, c.Status AS StatusId, c.PublishedDate
                           FROM Courses c
                           INNER JOIN Enrollments e ON e.CourseId = c.Id
                           WHERE e.StudentId = @StudentId
                           ORDER BY c.Title
                           """;

        var rows = await connection.QueryAsync<StudentCourseReadModel>(
            new CommandDefinition(sql, new { StudentId = studentId }, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT COUNT(DISTINCT StudentId)
                           FROM Enrollments
                           WHERE CourseId = @CourseId
                           """;

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(sql, new { CourseId = courseId }, cancellationToken: cancellationToken));
    }

    public async Task<PaginatedList<StudentReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Users
                                WHERE UserType = 'Student'
                                """;

        const string sql = """
                           SELECT Id, FirstName, LastName, Email, ProfilePictureUrl, LastLoginDate
                           FROM Users
                           WHERE UserType = 'Student'
                           ORDER BY LastName, FirstName, Id
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<StudentReadModel>(
            new CommandDefinition(
                sql,
                new
                {
                    pagination.PageSize,
                    Offset = pagination.SkipCount,
                },
                cancellationToken: cancellationToken));

        return new PaginatedList<StudentReadModel>(
            [.. rows],
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }
}