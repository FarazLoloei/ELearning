// <copyright file="SubmissionReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Data;
using Dapper;
using ELearning.Application.Submissions.Abstractions;
using ELearning.Application.Submissions.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

public class SubmissionReadRepository(ApplicationDbContext context) : ISubmissionReadRepository
{
    public async Task<SubmissionReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT Id, EnrollmentId, AssignmentId, Content, FileUrl, IsGraded, Score, Feedback, SubmittedDate, GradedById, GradedDate
                           FROM Submissions
                           WHERE Id = @Id
                           """;

        return await connection.QuerySingleOrDefaultAsync<SubmissionReadModel>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<SubmissionReadModel>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT Id, EnrollmentId, AssignmentId, Content, FileUrl, IsGraded, Score, Feedback, SubmittedDate, GradedById, GradedDate
                           FROM Submissions
                           WHERE AssignmentId = @AssignmentId
                           ORDER BY SubmittedDate DESC
                           """;

        var rows = await connection.QueryAsync<SubmissionReadModel>(
            new CommandDefinition(sql, new { AssignmentId = assignmentId }, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<IReadOnlyList<SubmissionReadModel>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT s.Id, s.EnrollmentId, s.AssignmentId, s.Content, s.FileUrl, s.IsGraded, s.Score, s.Feedback, s.SubmittedDate, s.GradedById, s.GradedDate
                           FROM Submissions s
                           INNER JOIN Enrollments e ON e.Id = s.EnrollmentId
                           WHERE e.StudentId = @StudentId
                           ORDER BY s.SubmittedDate DESC
                           """;

        var rows = await connection.QueryAsync<SubmissionReadModel>(
            new CommandDefinition(sql, new { StudentId = studentId }, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<SubmissionReadModel?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT s.Id, s.EnrollmentId, s.AssignmentId, s.Content, s.FileUrl, s.IsGraded, s.Score, s.Feedback, s.SubmittedDate, s.GradedById, s.GradedDate
                           FROM Submissions s
                           INNER JOIN Enrollments e ON e.Id = s.EnrollmentId
                           WHERE e.StudentId = @StudentId AND s.AssignmentId = @AssignmentId
                           ORDER BY s.SubmittedDate DESC
                           LIMIT 1
                           """;

        return await connection.QuerySingleOrDefaultAsync<SubmissionReadModel>(
            new CommandDefinition(sql, new { StudentId = studentId, AssignmentId = assignmentId }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<SubmissionReadModel>> GetUngradedSubmissionsAsync(CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT Id, EnrollmentId, AssignmentId, Content, FileUrl, IsGraded, Score, Feedback, SubmittedDate, GradedById, GradedDate
                           FROM Submissions
                           WHERE IsGraded = 0
                           ORDER BY SubmittedDate
                           """;

        var rows = await connection.QueryAsync<SubmissionReadModel>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));

        return [.. rows];
    }

    public async Task<PaginatedList<SubmissionReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string countSql = """
                                SELECT COUNT(*)
                                FROM Submissions
                                """;

        const string sql = """
                           SELECT Id, EnrollmentId, AssignmentId, Content, FileUrl, IsGraded, Score, Feedback, SubmittedDate, GradedById, GradedDate
                           FROM Submissions
                           ORDER BY SubmittedDate DESC, Id
                           LIMIT @PageSize OFFSET @Offset
                           """;

        var totalCount = await connection.QuerySingleAsync<int>(
            new CommandDefinition(countSql, cancellationToken: cancellationToken));

        var rows = await connection.QueryAsync<SubmissionReadModel>(
            new CommandDefinition(
                sql,
                new
                {
                    pagination.PageSize,
                    Offset = pagination.SkipCount,
                },
                cancellationToken: cancellationToken));

        return new PaginatedList<SubmissionReadModel>(
            [.. rows],
            totalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }
}