using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Progress Repository Implementation
public class ProgressRepository : IProgressRepository
{
    private readonly ApplicationDbContext _context;

    public ProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Progress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Progresses
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Progresses
            .Where(p => p.EnrollmentId == enrollmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default)
    {
        return await _context.Progresses
            .SingleOrDefaultAsync(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId, cancellationToken);
    }

    public async Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.ProgressRecords)
            .SingleOrDefaultAsync(e => e.Id == enrollmentId, cancellationToken);

        if (enrollment == null)
        {
            return 0;
        }

        // Get the course
        var course = await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .SingleOrDefaultAsync(c => c.Id == enrollment.CourseId, cancellationToken);

        if (course == null)
        {
            return 0;
        }

        // Count total lessons in the course
        var totalLessons = course.Modules.Sum(m => m.Lessons.Count);
        if (totalLessons == 0)
        {
            return 0;
        }

        // Count completed lessons
        var completedLessons = enrollment.ProgressRecords
            .Count(p => p.Status == ProgressStatus.Completed);

        return (double)completedLessons / totalLessons * 100;
    }

    public async Task AddAsync(Progress progress, CancellationToken cancellationToken = default)
    {
        await _context.Progresses.AddAsync(progress, cancellationToken);
    }

    public Task UpdateAsync(Progress progress, CancellationToken cancellationToken = default)
    {
        _context.Entry(progress).State = EntityState.Modified;
        return Task.CompletedTask;
    }
}
