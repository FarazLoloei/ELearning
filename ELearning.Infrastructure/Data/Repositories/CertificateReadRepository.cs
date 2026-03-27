// <copyright file="CertificateReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using Dapper;
using ELearning.Application.Certificates.Abstractions;
using ELearning.Application.Certificates.ReadModels;
using Microsoft.EntityFrameworkCore;

public sealed class CertificateReadRepository(ApplicationDbContext context) : ICertificateReadRepository
{
    public async Task<CertificateReadModel?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT c.Id,
                                  c.CertificateCode,
                                  c.EnrollmentId,
                                  c.StudentId,
                                  u.FirstName || ' ' || u.LastName AS StudentName,
                                  c.CourseId,
                                  crs.Title AS CourseTitle,
                                  c.IssuedOnUtc
                           FROM Certificates c
                           INNER JOIN Users u ON u.Id = c.StudentId
                           INNER JOIN Courses crs ON crs.Id = c.CourseId
                           WHERE c.EnrollmentId = @EnrollmentId
                           """;

        return await connection.QuerySingleOrDefaultAsync<CertificateReadModel>(
            new CommandDefinition(sql, new { EnrollmentId = enrollmentId }, cancellationToken: cancellationToken));
    }

    public async Task<CertificateReadModel?> GetByCodeAsync(string certificateCode, CancellationToken cancellationToken = default)
    {
        var connection = context.Database.GetDbConnection();
        await connection.EnsureOpenAsync(cancellationToken);

        const string sql = """
                           SELECT c.Id,
                                  c.CertificateCode,
                                  c.EnrollmentId,
                                  c.StudentId,
                                  u.FirstName || ' ' || u.LastName AS StudentName,
                                  c.CourseId,
                                  crs.Title AS CourseTitle,
                                  c.IssuedOnUtc
                           FROM Certificates c
                           INNER JOIN Users u ON u.Id = c.StudentId
                           INNER JOIN Courses crs ON crs.Id = c.CourseId
                           WHERE c.CertificateCode = @CertificateCode
                           """;

        return await connection.QuerySingleOrDefaultAsync<CertificateReadModel>(
            new CommandDefinition(sql, new { CertificateCode = certificateCode }, cancellationToken: cancellationToken));
    }
}
