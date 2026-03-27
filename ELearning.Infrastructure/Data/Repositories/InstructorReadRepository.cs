// <copyright file="InstructorReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using Dapper;
using ELearning.Application.Instructors.ReadModels;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class InstructorReadRepository(ApplicationDbContext context) : IInstructorReadRepository
{
    public async Task<InstructorReadModel?> GetByIdAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT u.Id,
                                  u.FirstName,
                                  u.LastName,
                                  u.Email,
                                  u.Bio,
                                  u.Expertise,
                                  u.ProfilePictureUrl,
                                  COUNT(DISTINCT c.Id) AS TotalCourses,
                                  COUNT(DISTINCT e.StudentId) AS TotalStudents,
                                  COALESCE(SUM(c.AverageRatingValue * c.NumberOfRatings), 0) AS WeightedRatingsSum,
                                  COALESCE(SUM(c.NumberOfRatings), 0) AS TotalRatingsCount
                           FROM Users u
                           LEFT JOIN Courses c ON c.InstructorId = u.Id
                           LEFT JOIN Enrollments e ON e.CourseId = c.Id
                           WHERE u.Id = @Id AND u.UserType = 'Instructor'
                           GROUP BY u.Id, u.FirstName, u.LastName, u.Email, u.Bio, u.Expertise, u.ProfilePictureUrl
                           """;

        var row = await connection.QuerySingleOrDefaultAsync<InstructorSummaryRow>(
            new CommandDefinition(sql, new { Id = instructorId }, cancellationToken: cancellationToken));

        return row is null ? null : MapToInstructorReadModel(row);
    }

    public async Task<IReadOnlyList<InstructorReadModel>> GetTopInstructorsAsync(int count, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT u.Id,
                                  u.FirstName,
                                  u.LastName,
                                  u.Email,
                                  u.Bio,
                                  u.Expertise,
                                  u.ProfilePictureUrl,
                                  COUNT(DISTINCT c.Id) AS TotalCourses,
                                  COUNT(DISTINCT e.StudentId) AS TotalStudents,
                                  COALESCE(SUM(c.AverageRatingValue * c.NumberOfRatings), 0) AS WeightedRatingsSum,
                                  COALESCE(SUM(c.NumberOfRatings), 0) AS TotalRatingsCount
                           FROM Users u
                           LEFT JOIN Courses c ON c.InstructorId = u.Id
                           LEFT JOIN Enrollments e ON e.CourseId = c.Id
                           WHERE u.UserType = 'Instructor'
                           GROUP BY u.Id, u.FirstName, u.LastName, u.Email, u.Bio, u.Expertise, u.ProfilePictureUrl
                           ORDER BY COUNT(DISTINCT c.Id) DESC, u.LastName, u.FirstName
                           LIMIT @Count
                           """;

        var rows = await connection.QueryAsync<InstructorSummaryRow>(
            new CommandDefinition(sql, new { Count = count }, cancellationToken: cancellationToken));

        return rows.Select(MapToInstructorReadModel).ToList();
    }

    public async Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT COUNT(DISTINCT e.StudentId)
                           FROM Enrollments e
                           INNER JOIN Courses c ON c.Id = e.CourseId
                           WHERE c.InstructorId = @InstructorId
                           """;

        return await connection.QuerySingleAsync<int>(
            new CommandDefinition(sql, new { InstructorId = instructorId }, cancellationToken: cancellationToken));
    }

    public async Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT AverageRatingValue AS Rating, NumberOfRatings AS RatingCount
                           FROM Courses
                           WHERE InstructorId = @InstructorId
                           """;

        var ratings = await connection.QueryAsync<RatingRow>(
            new CommandDefinition(sql, new { InstructorId = instructorId }, cancellationToken: cancellationToken));

        var ratingRows = ratings.ToList();
        var totalRatingsCount = ratingRows.Sum(r => r.RatingCount);
        if (totalRatingsCount == 0)
        {
            return 0;
        }

        var weightedSum = ratingRows.Sum(r => r.Rating * r.RatingCount);
        return weightedSum / totalRatingsCount;
    }

    public async Task<InstructorWithCoursesReadModel?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string instructorSql = """
                                     SELECT u.Id,
                                            u.FirstName,
                                            u.LastName,
                                            u.Email,
                                            u.Bio,
                                            u.Expertise,
                                            u.ProfilePictureUrl,
                                            COUNT(DISTINCT c.Id) AS TotalCourses,
                                            COUNT(DISTINCT e.StudentId) AS TotalStudents,
                                            COALESCE(SUM(c.AverageRatingValue * c.NumberOfRatings), 0) AS WeightedRatingsSum,
                                            COALESCE(SUM(c.NumberOfRatings), 0) AS TotalRatingsCount
                                     FROM Users u
                                     LEFT JOIN Courses c ON c.InstructorId = u.Id
                                     LEFT JOIN Enrollments e ON e.CourseId = c.Id
                                     WHERE u.Id = @InstructorId AND u.UserType = 'Instructor'
                                     GROUP BY u.Id, u.FirstName, u.LastName, u.Email, u.Bio, u.Expertise, u.ProfilePictureUrl
                                     """;

        var instructor = await connection.QuerySingleOrDefaultAsync<InstructorSummaryRow>(
            new CommandDefinition(instructorSql, new { InstructorId = instructorId }, cancellationToken: cancellationToken));

        if (instructor is null)
        {
            return null;
        }

        const string coursesSql = """
                                  SELECT c.Id,
                                         c.Title,
                                         c.Category AS CategoryId,
                                         c.Status AS StatusId,
                                         c.PublishedDate,
                                         c.createdAtUTC AS CreatedAt,
                                         COUNT(DISTINCT e.StudentId) AS EnrollmentsCount
                                  FROM Courses c
                                  LEFT JOIN Enrollments e ON e.CourseId = c.Id
                                  WHERE c.InstructorId = @InstructorId
                                  GROUP BY c.Id, c.Title, c.Category, c.Status, c.PublishedDate, c.createdAtUTC
                                  ORDER BY c.createdAtUTC DESC
                                  """;

        var courses = await connection.QueryAsync<InstructorCourseRow>(
            new CommandDefinition(coursesSql, new { InstructorId = instructorId }, cancellationToken: cancellationToken));

        var courseReadModels = courses.Select(MapToInstructorCourseReadModel).ToList();

        return new InstructorWithCoursesReadModel(
            instructor.Id,
            instructor.FirstName,
            instructor.LastName,
            instructor.Email,
            instructor.Bio,
            instructor.Expertise,
            instructor.ProfilePictureUrl ?? string.Empty,
            CalculateAverageRating(instructor.WeightedRatingsSum, instructor.TotalRatingsCount),
            instructor.TotalStudents,
            instructor.TotalCourses,
            courseReadModels);
    }

    public async Task<PaginatedList<InstructorReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Users
                                WHERE UserType = 'Instructor'
                                """;

        const string sql = """
                           SELECT u.Id,
                                  u.FirstName,
                                  u.LastName,
                                  u.Email,
                                  u.Bio,
                                  u.Expertise,
                                  u.ProfilePictureUrl,
                                  COUNT(DISTINCT c.Id) AS TotalCourses,
                                  COUNT(DISTINCT e.StudentId) AS TotalStudents,
                                  COALESCE(SUM(c.AverageRatingValue * c.NumberOfRatings), 0) AS WeightedRatingsSum,
                                  COALESCE(SUM(c.NumberOfRatings), 0) AS TotalRatingsCount
                           FROM Users u
                           LEFT JOIN Courses c ON c.InstructorId = u.Id
                           LEFT JOIN Enrollments e ON e.CourseId = c.Id
                           WHERE u.UserType = 'Instructor'
                           GROUP BY u.Id, u.FirstName, u.LastName, u.Email, u.Bio, u.Expertise, u.ProfilePictureUrl
                           ORDER BY u.LastName, u.FirstName
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<InstructorSummaryRow>(
            new CommandDefinition(
                sql,
                new
                {
                    PageSize = pagination.PageSize,
                    Offset = pagination.SkipCount,
                },
                cancellationToken: cancellationToken));

        var items = rows.Select(MapToInstructorReadModel).ToList();

        return new PaginatedList<InstructorReadModel>(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    private static InstructorReadModel MapToInstructorReadModel(InstructorSummaryRow row) =>
        new(
            row.Id,
            row.FirstName,
            row.LastName,
            row.Email,
            row.Bio,
            row.Expertise,
            row.ProfilePictureUrl ?? string.Empty,
            CalculateAverageRating(row.WeightedRatingsSum, row.TotalRatingsCount),
            row.TotalStudents,
            row.TotalCourses);

    private static InstructorCourseReadModel MapToInstructorCourseReadModel(InstructorCourseRow row) =>
        new(
            row.Id,
            row.Title,
            row.CategoryId,
            row.EnrollmentsCount,
            row.StatusId,
            row.PublishedDate,
            row.CreatedAt);

    private static decimal CalculateAverageRating(decimal weightedRatingsSum, int totalRatingsCount) =>
        totalRatingsCount == 0 ? 0 : weightedRatingsSum / totalRatingsCount;

    private sealed record InstructorSummaryRow(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        string Bio,
        string Expertise,
        string? ProfilePictureUrl,
        int TotalCourses,
        int TotalStudents,
        decimal WeightedRatingsSum,
        int TotalRatingsCount);

    private sealed record InstructorCourseRow(
        Guid Id,
        string Title,
        int CategoryId,
        int StatusId,
        DateTime CreatedAt,
        DateTime? PublishedDate,
        int EnrollmentsCount);

    private sealed record RatingRow(decimal Rating, int RatingCount);
}
