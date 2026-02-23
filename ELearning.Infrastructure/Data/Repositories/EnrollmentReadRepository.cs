using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class EnrollmentReadRepository(ApplicationDbContext context) : IEnrollmentReadRepository
{
    public async Task<PaginatedList<Enrollment>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var query = context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(e => e.CreatedAt())
            .Skip(pagination.SkipCount)
            .Take(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<Enrollment>(items, totalCount, pagination.PageNumber, pagination.PageSize);
    }

    public async Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default)
    {
        return await context.Enrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCourseEnrollmentCountAsync(Guid courseId, CancellationToken cancellationToken = default) =>
        await context.Enrollments
            .AsNoTracking()
            .CountAsync(e => e.CourseId == courseId, cancellationToken);

    public async Task<Dictionary<Guid, int>> GetCourseEnrollmentCountsAsync(
        IReadOnlyCollection<Guid> courseIds,
        CancellationToken cancellationToken = default)
    {
        if (courseIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        return await context.Enrollments
            .AsNoTracking()
            .Where(e => courseIds.Contains(e.CourseId))
            .GroupBy(e => e.CourseId)
            .Select(group => new { CourseId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.CourseId, item => item.Count, cancellationToken);
    }
}
