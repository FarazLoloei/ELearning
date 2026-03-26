// <copyright file="EnrollmentRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

public class EnrollmentRepository(ApplicationDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .Include(e => e.ProgressRecords)
            .Include(e => e.Submissions)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AddAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        await context.Enrollments.AddAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        var entry = context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            context.Enrollments.Attach(entity);
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        context.Enrollments.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .Include(e => e.Submissions)
            .Include(e => e.ProgressRecords)
            .Where(e => e.StudentId == studentId && e.CourseId == courseId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .Include(e => e.Submissions)
            .Include(e => e.ProgressRecords)
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .Include(e => e.Submissions)
            .Include(e => e.ProgressRecords)
            .FirstOrDefaultAsync(e => e.Submissions.Any(s => s.Id == submissionId), cancellationToken);
    }

    public async Task<bool> HasAnyForCourseAsync(Guid courseId, CancellationToken cancellationToken) =>
        await context.Enrollments
            .AsNoTracking()
            .AnyAsync(e => e.CourseId == courseId, cancellationToken);
}
