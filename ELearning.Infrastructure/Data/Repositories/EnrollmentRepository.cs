// <copyright file="EnrollmentRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

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

    public async Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        var entry = context.Entry(entity);
        if (entry.State == EntityState.Detached)
        {
            context.Enrollments.Attach(entity);
            entry = context.Entry(entity);
        }

        var rowVersionEntry = entry.Property(nameof(Enrollment.RowVersion));
        if (rowVersionEntry.OriginalValue is null && rowVersionEntry.CurrentValue is not null)
        {
            rowVersionEntry.OriginalValue = rowVersionEntry.CurrentValue;
        }

        await this.TrackNewEnrollmentContentAsync(entity, cancellationToken);
    }

    private async Task TrackNewEnrollmentContentAsync(Enrollment enrollment, CancellationToken cancellationToken)
    {
        foreach (var progress in enrollment.ProgressRecords)
        {
            var progressEntry = context.Entry(progress);
            await TrackAsAddedWhenMissingAsync(
                progressEntry,
                () => context.Progresses.AnyAsync(existingProgress => existingProgress.Id == progress.Id, cancellationToken));
        }

        foreach (var submission in enrollment.Submissions)
        {
            var submissionEntry = context.Entry(submission);
            await TrackAsAddedWhenMissingAsync(
                submissionEntry,
                () => context.Submissions.AnyAsync(existingSubmission => existingSubmission.Id == submission.Id, cancellationToken));
        }
    }

    private static async Task TrackAsAddedWhenMissingAsync(EntityEntry entry, Func<Task<bool>> existsInDatabase)
    {
        if (entry.State == EntityState.Added)
        {
            return;
        }

        if (entry.State is not (EntityState.Detached or EntityState.Modified))
        {
            return;
        }

        if (!await existsInDatabase())
        {
            entry.State = EntityState.Added;
        }
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
