using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class EnrollmentRepository(ApplicationDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .Include(e => e.ProgressRecords)
            .Include(e => e.Submissions)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> ListAllAsync(CancellationToken cancellationToken) =>
        await context.Enrollments.ToListAsync(cancellationToken);

    public async Task AddAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        await context.Enrollments.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Enrollment entity, CancellationToken cancellationToken)
    {
        context.Enrollments.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId && e.CourseId == courseId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Enrollment>> GetRecentEnrollmentsAsync(int count, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .OrderByDescending(e => e.CreatedAt())
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetEnrollmentsCountAsync(CancellationToken cancellationToken) =>
        await context.Enrollments.CountAsync(cancellationToken);

    public async Task<IReadOnlyList<Enrollment>> GetCompletedEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
            .ToListAsync(cancellationToken);
    }
}