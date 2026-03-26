// <copyright file="EnrollmentReadRepository.cs" company="FarazLoloei">
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

/// <summary>
/// Reads enrollment projections directly from the owned database.
/// </summary>
public class EnrollmentReadRepository(ApplicationDbContext context) : IEnrollmentReadRepository
{
    /// <summary>
    /// Gets a single enrollment detail projection by identifier.
    /// </summary>
    /// <param name="id">The enrollment identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The enrollment detail projection when found; otherwise <see langword="null"/>.</returns>
    public async Task<EnrollmentDetailReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT e.Id,
                                  e.StudentId,
                                  u.FirstName AS StudentFirstName,
                                  u.LastName AS StudentLastName,
                                  e.CourseId,
                                  c.Title AS CourseTitle,
                                  e.Status AS StatusId,
                                  e.createdAtUTC AS EnrollmentDate,
                                  e.CompletedDateUTC AS CompletedDate,
                                  CASE
                                      WHEN COALESCE(tl.TotalLessons, 0) = 0 THEN 0.0
                                      ELSE (CAST(COALESCE(cl.CompletedLessons, 0) AS FLOAT) / CAST(tl.TotalLessons AS FLOAT)) * 100.0
                                  END AS CompletionPercentage,
                                  e.CourseRatingValue AS CourseRating,
                                  e.Review
                           FROM Enrollments e
                           INNER JOIN Users u ON u.Id = e.StudentId
                           INNER JOIN Courses c ON c.Id = e.CourseId
                           LEFT JOIN (
                               SELECT m.CourseId,
                                      COUNT(l.Id) AS TotalLessons
                               FROM Modules m
                               LEFT JOIN Lessons l ON l.ModuleId = m.Id
                               GROUP BY m.CourseId
                           ) tl ON tl.CourseId = e.CourseId
                           LEFT JOIN (
                               SELECT EnrollmentId,
                                      COUNT(DISTINCT LessonId) AS CompletedLessons
                               FROM Progresses
                               WHERE Status = 3
                               GROUP BY EnrollmentId
                           ) cl ON cl.EnrollmentId = e.Id
                           WHERE e.Id = @Id
                           """;

        var row = await connection.QuerySingleOrDefaultAsync<EnrollmentDetailRow>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));

        if (row is null)
        {
            return null;
        }

        var enrollmentIds = new[] { id };
        var lessonProgressLookup = await this.LoadLessonProgressLookupAsync(connection, enrollmentIds, cancellationToken);
        var submissionLookup = await this.LoadSubmissionLookupAsync(connection, enrollmentIds, cancellationToken);

        return MapToDetailReadModel(
            row,
            lessonProgressLookup.GetValueOrDefault(id, Array.Empty<LessonProgressReadModel>()),
            submissionLookup.GetValueOrDefault(id, Array.Empty<EnrollmentSubmissionReadModel>()));
    }

    /// <summary>
    /// Gets a paged list of enrollments for a specific student.
    /// </summary>
    /// <param name="studentId">The student identifier.</param>
    /// <param name="pagination">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of enrollment summary projections.</returns>
    public async Task<PaginatedList<EnrollmentSummaryReadModel>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Enrollments
                                WHERE StudentId = @StudentId
                                """;

        const string sql = """
                           SELECT e.Id,
                                  e.StudentId,
                                  u.FirstName AS StudentFirstName,
                                  u.LastName AS StudentLastName,
                                  e.CourseId,
                                  c.Title AS CourseTitle,
                                  e.Status AS StatusId,
                                  e.createdAtUTC AS EnrollmentDate,
                                  e.CompletedDateUTC AS CompletedDate,
                                  CASE
                                      WHEN COALESCE(tl.TotalLessons, 0) = 0 THEN 0.0
                                      ELSE (CAST(COALESCE(cl.CompletedLessons, 0) AS FLOAT) / CAST(tl.TotalLessons AS FLOAT)) * 100.0
                                  END AS CompletionPercentage
                           FROM Enrollments e
                           INNER JOIN Users u ON u.Id = e.StudentId
                           INNER JOIN Courses c ON c.Id = e.CourseId
                           LEFT JOIN (
                               SELECT m.CourseId,
                                      COUNT(l.Id) AS TotalLessons
                               FROM Modules m
                               LEFT JOIN Lessons l ON l.ModuleId = m.Id
                               GROUP BY m.CourseId
                           ) tl ON tl.CourseId = e.CourseId
                           LEFT JOIN (
                               SELECT EnrollmentId,
                                      COUNT(DISTINCT LessonId) AS CompletedLessons
                               FROM Progresses
                               WHERE Status = 3
                               GROUP BY EnrollmentId
                           ) cl ON cl.EnrollmentId = e.Id
                           WHERE e.StudentId = @StudentId
                           ORDER BY e.createdAtUTC DESC, e.Id
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var parameters = new
        {
            StudentId = studentId,
            pagination.PageSize,
            Offset = pagination.SkipCount,
        };

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, new { StudentId = studentId }, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<EnrollmentSummaryRow>(
            new CommandDefinition(sql, parameters, cancellationToken: cancellationToken));

        return new PaginatedList<EnrollmentSummaryReadModel>(
            [.. rows.Select(MapToSummaryReadModel)],
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    /// <summary>
    /// Gets a paged list of enrollment detail projections.
    /// </summary>
    /// <param name="pagination">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of enrollment detail projections.</returns>
    public async Task<PaginatedList<EnrollmentDetailReadModel>> ListAsync(
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Enrollments
                                """;

        const string sql = """
                           SELECT e.Id,
                                  e.StudentId,
                                  u.FirstName AS StudentFirstName,
                                  u.LastName AS StudentLastName,
                                  e.CourseId,
                                  c.Title AS CourseTitle,
                                  e.Status AS StatusId,
                                  e.createdAtUTC AS EnrollmentDate,
                                  e.CompletedDateUTC AS CompletedDate,
                                  CASE
                                      WHEN COALESCE(tl.TotalLessons, 0) = 0 THEN 0.0
                                      ELSE (CAST(COALESCE(cl.CompletedLessons, 0) AS FLOAT) / CAST(tl.TotalLessons AS FLOAT)) * 100.0
                                  END AS CompletionPercentage,
                                  e.CourseRatingValue AS CourseRating,
                                  e.Review
                           FROM Enrollments e
                           INNER JOIN Users u ON u.Id = e.StudentId
                           INNER JOIN Courses c ON c.Id = e.CourseId
                           LEFT JOIN (
                               SELECT m.CourseId,
                                      COUNT(l.Id) AS TotalLessons
                               FROM Modules m
                               LEFT JOIN Lessons l ON l.ModuleId = m.Id
                               GROUP BY m.CourseId
                           ) tl ON tl.CourseId = e.CourseId
                           LEFT JOIN (
                               SELECT EnrollmentId,
                                      COUNT(DISTINCT LessonId) AS CompletedLessons
                               FROM Progresses
                               WHERE Status = 3
                               GROUP BY EnrollmentId
                           ) cl ON cl.EnrollmentId = e.Id
                           ORDER BY e.createdAtUTC DESC, e.Id
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        var rows = (await connection.QueryAsync<EnrollmentDetailRow>(
            new CommandDefinition(
                sql,
                new
                {
                    pagination.PageSize,
                    Offset = pagination.SkipCount,
                },
                cancellationToken: cancellationToken))).ToList();

        if (rows.Count == 0)
        {
            return new PaginatedList<EnrollmentDetailReadModel>(
                [],
                totalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }

        var enrollmentIds = rows.Select(row => row.Id).ToArray();
        var lessonProgressLookup = await this.LoadLessonProgressLookupAsync(connection, enrollmentIds, cancellationToken);
        var submissionLookup = await this.LoadSubmissionLookupAsync(connection, enrollmentIds, cancellationToken);

        var items = rows.Select(row => MapToDetailReadModel(
            row,
            lessonProgressLookup.GetValueOrDefault(row.Id, Array.Empty<LessonProgressReadModel>()),
            submissionLookup.GetValueOrDefault(row.Id, Array.Empty<EnrollmentSubmissionReadModel>()))).ToList();

        return new PaginatedList<EnrollmentDetailReadModel>(
            items,
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private static EnrollmentSummaryReadModel MapToSummaryReadModel(EnrollmentSummaryRow row) =>
        new(
            row.Id,
            row.StudentId,
            BuildStudentName(row.StudentFirstName, row.StudentLastName),
            row.CourseId,
            row.CourseTitle,
            MapEnrollmentStatus(row.StatusId),
            row.EnrollmentDate,
            row.CompletedDate,
            row.CompletionPercentage);

    private static EnrollmentDetailReadModel MapToDetailReadModel(
        EnrollmentDetailRow row,
        IReadOnlyList<LessonProgressReadModel> lessonProgress,
        IReadOnlyList<EnrollmentSubmissionReadModel> submissions) =>
        new(
            row.Id,
            row.StudentId,
            BuildStudentName(row.StudentFirstName, row.StudentLastName),
            row.CourseId,
            row.CourseTitle,
            MapEnrollmentStatus(row.StatusId),
            row.EnrollmentDate,
            row.CompletedDate,
            row.CompletionPercentage,
            lessonProgress,
            submissions,
            row.CourseRating,
            row.Review);

    private static string BuildStudentName(string firstName, string lastName) =>
        $"{firstName} {lastName}".Trim();

    private static string MapEnrollmentStatus(int statusId) =>
        statusId switch
        {
            1 => "Active",
            2 => "Paused",
            3 => "Completed",
            4 => "Abandoned",
            _ => "Unknown",
        };

    private static string MapProgressStatus(int statusId) =>
        statusId switch
        {
            1 => "NotStarted",
            2 => "InProgress",
            3 => "Completed",
            _ => "Unknown",
        };

    private static Dictionary<Guid, IReadOnlyList<LessonProgressReadModel>> GroupLessonProgressByEnrollment(
        IEnumerable<LessonProgressRow> rows) =>
        rows.GroupBy(row => row.EnrollmentId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<LessonProgressReadModel>)[.. group.Select(row => new LessonProgressReadModel(
                    row.LessonId,
                    row.LessonTitle,
                    MapProgressStatus(row.StatusId),
                    row.CompletedDate,
                    row.TimeSpentSeconds))]);

    private static Dictionary<Guid, IReadOnlyList<EnrollmentSubmissionReadModel>> GroupSubmissionsByEnrollment(
        IEnumerable<EnrollmentSubmissionRow> rows) =>
        rows.GroupBy(row => row.EnrollmentId)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<EnrollmentSubmissionReadModel>)[.. group.Select(row => new EnrollmentSubmissionReadModel(
                    row.Id,
                    row.AssignmentId,
                    row.AssignmentTitle,
                    row.SubmittedDate,
                    row.IsGraded,
                    row.Score,
                    row.MaxPoints))]);

    private async Task<Dictionary<Guid, IReadOnlyList<LessonProgressReadModel>>> LoadLessonProgressLookupAsync(
        IDbConnection connection,
        IReadOnlyCollection<Guid> enrollmentIds,
        CancellationToken cancellationToken)
    {
        if (enrollmentIds.Count == 0)
        {
            return [];
        }

        const string sql = """
                           SELECT e.Id AS EnrollmentId,
                                  l.Id AS LessonId,
                                  l.Title AS LessonTitle,
                                  COALESCE(p.Status, 1) AS StatusId,
                                  p.CompletedDate,
                                  COALESCE(p.TimeSpentSeconds, 0) AS TimeSpentSeconds
                           FROM Enrollments e
                           INNER JOIN Modules m ON m.CourseId = e.CourseId
                           INNER JOIN Lessons l ON l.ModuleId = m.Id
                           LEFT JOIN Progresses p ON p.EnrollmentId = e.Id AND p.LessonId = l.Id
                           WHERE e.Id IN @EnrollmentIds
                           ORDER BY e.Id, m."Order", l."Order", l.Title
                           """;

        var rows = await connection.QueryAsync<LessonProgressRow>(
            new CommandDefinition(sql, new { EnrollmentIds = enrollmentIds }, cancellationToken: cancellationToken));

        return GroupLessonProgressByEnrollment(rows);
    }

    private async Task<Dictionary<Guid, IReadOnlyList<EnrollmentSubmissionReadModel>>> LoadSubmissionLookupAsync(
        IDbConnection connection,
        IReadOnlyCollection<Guid> enrollmentIds,
        CancellationToken cancellationToken)
    {
        if (enrollmentIds.Count == 0)
        {
            return [];
        }

        const string sql = """
                           SELECT s.EnrollmentId,
                                  s.Id,
                                  s.AssignmentId,
                                  a.Title AS AssignmentTitle,
                                  s.SubmittedDate,
                                  s.IsGraded,
                                  s.Score,
                                  a.MaxPoints
                           FROM Submissions s
                           INNER JOIN Assignments a ON a.Id = s.AssignmentId
                           WHERE s.EnrollmentId IN @EnrollmentIds
                           ORDER BY s.EnrollmentId, s.SubmittedDate DESC, s.Id
                           """;

        var rows = await connection.QueryAsync<EnrollmentSubmissionRow>(
            new CommandDefinition(sql, new { EnrollmentIds = enrollmentIds }, cancellationToken: cancellationToken));

        return GroupSubmissionsByEnrollment(rows);
    }

    private sealed record EnrollmentSummaryRow(
        Guid Id,
        Guid StudentId,
        string StudentFirstName,
        string StudentLastName,
        Guid CourseId,
        string CourseTitle,
        int StatusId,
        DateTime EnrollmentDate,
        DateTime? CompletedDate,
        double CompletionPercentage);

    private sealed record EnrollmentDetailRow(
        Guid Id,
        Guid StudentId,
        string StudentFirstName,
        string StudentLastName,
        Guid CourseId,
        string CourseTitle,
        int StatusId,
        DateTime EnrollmentDate,
        DateTime? CompletedDate,
        double CompletionPercentage,
        decimal? CourseRating,
        string? Review);

    private sealed record LessonProgressRow(
        Guid EnrollmentId,
        Guid LessonId,
        string LessonTitle,
        int StatusId,
        DateTime? CompletedDate,
        int TimeSpentSeconds);

    private sealed record EnrollmentSubmissionRow(
        Guid EnrollmentId,
        Guid Id,
        Guid AssignmentId,
        string AssignmentTitle,
        DateTime SubmittedDate,
        bool IsGraded,
        int? Score,
        int MaxPoints);
}
