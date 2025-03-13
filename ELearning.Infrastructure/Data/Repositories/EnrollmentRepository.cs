using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class EnrollmentRepository(ApplicationDbContext context) : IEnrollmentRepository
{
    public async Task<Enrollment> GetByIdAsync(Guid id)
    {
        return await context.Enrollments
            .Include(e => e.ProgressRecords)
            .Include(e => e.Submissions)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<Enrollment>> ListAllAsync()
    {
        return await context.Enrollments.ToListAsync();
    }

    public async Task AddAsync(Enrollment entity)
    {
        await context.Enrollments.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Enrollment entity)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Enrollment entity)
    {
        context.Enrollments.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Enrollment> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId)
    {
        return await context.Enrollments
            .Where(e => e.StudentId == studentId && e.CourseId == courseId)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId)
    {
        return await context.Enrollments
            .Where(e => e.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(Guid courseId)
    {
        return await context.Enrollments
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetRecentEnrollmentsAsync(int count)
    {
        return await context.Enrollments
            .OrderByDescending(e => e.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> GetEnrollmentsCountAsync()
    {
        return await context.Enrollments.CountAsync();
    }

    public async Task<IReadOnlyList<Enrollment>> GetCompletedEnrollmentsAsync(Guid studentId)
    {
        return await context.Enrollments
            .Where(e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed)
            .ToListAsync();
    }
}