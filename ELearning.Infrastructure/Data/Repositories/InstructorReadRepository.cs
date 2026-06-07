// <copyright file="InstructorReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using System.Globalization;
using Dapper;
using ELearning.Application.Instructors.ReadModels;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class InstructorReadRepository(ApplicationDbContext context, ISqlDialect sqlDialect) : IInstructorReadRepository
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
        var fetchFirstClause = sqlDialect.FetchFirst("@Count");

        var sql = $$"""
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
                    {{fetchFirstClause}}
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

        var instructor = await connection.QuerySingleOrDefaultAsync(
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
                                         c.PublishedDate AS PublishedDate,
                                         c.createdAtUTC AS CreatedAtUtc,
                                         COUNT(DISTINCT e.StudentId) AS EnrollmentsCount
                                  FROM Courses c
                                  LEFT JOIN Enrollments e ON e.CourseId = c.Id
                                  WHERE c.InstructorId = @InstructorId
                                  GROUP BY c.Id, c.Title, c.Category, c.Status, c.PublishedDate, c.createdAtUTC
                                  ORDER BY c.createdAtUTC DESC
                                  """;

        var courses = await connection.QueryAsync(
            new CommandDefinition(coursesSql, new { InstructorId = instructorId }, cancellationToken: cancellationToken));

        var courseReadModels = courses.Select(row => new InstructorCourseReadModel(
            ReadGuid(row.Id),
            ReadString(row.Title),
            ReadInt32(row.CategoryId),
            ReadInt32(row.EnrollmentsCount),
            ReadInt32(row.StatusId),
            ReadNullableDateTime(row.PublishedDate),
            ReadDateTime(row.CreatedAtUtc))).ToList();

        return new InstructorWithCoursesReadModel(
            ReadGuid(instructor.Id),
            ReadString(instructor.FirstName),
            ReadString(instructor.LastName),
            ReadString(instructor.Email),
            ReadString(instructor.Bio),
            ReadString(instructor.Expertise),
            ReadNullableString(instructor.ProfilePictureUrl),
            CalculateAverageRating(ReadDecimal(instructor.WeightedRatingsSum), ReadInt32(instructor.TotalRatingsCount)),
            ReadInt32(instructor.TotalStudents),
            ReadInt32(instructor.TotalCourses),
            courseReadModels);
    }

    public async Task<PaginatedList<InstructorReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);
        var pagingClause = sqlDialect.Page();

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Users
                                WHERE UserType = 'Instructor'
                                """;

        var sql = $$"""
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
                    {{pagingClause}}
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

    private static decimal CalculateAverageRating(decimal weightedRatingsSum, int totalRatingsCount) =>
        totalRatingsCount == 0 ? 0 : weightedRatingsSum / totalRatingsCount;

    private static Guid ReadGuid(object value) =>
        value switch
        {
            Guid guid => guid,
            string text => Guid.Parse(text),
            _ => Guid.Parse(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty),
        };

    private static string ReadString(object? value) =>
        value?.ToString() ?? string.Empty;

    private static string? ReadNullableString(object? value) =>
        value?.ToString();

    private static int ReadInt32(object value) =>
        Convert.ToInt32(value, CultureInfo.InvariantCulture);

    private static decimal ReadDecimal(object value) =>
        Convert.ToDecimal(value, CultureInfo.InvariantCulture);

    private static DateTime ReadDateTime(object value) =>
        value switch
        {
            DateTime dateTime => dateTime,
            string text => DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
            _ => Convert.ToDateTime(value, CultureInfo.InvariantCulture),
        };

    private static DateTime? ReadNullableDateTime(object? value) =>
        value is null or DBNull ? null : ReadDateTime(value);

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

    private sealed record RatingRow(decimal Rating, int RatingCount);
}
