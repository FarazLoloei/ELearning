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

    public async Task<Progress?> GetByIdAsync(Guid id)
    {
        return await _context.Progresses
            .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId)
    {
        return await _context.Progresses
            .Where(p => p.EnrollmentId == enrollmentId)
            .ToListAsync();
    }

    public async Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId)
    {
        return await _context.Progresses
            .SingleOrDefaultAsync(p => p.EnrollmentId == enrollmentId && p.LessonId == lessonId);
    }

    public async Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId)
    {
        var enrollment = await _context.Enrollments
            .Include(e => e.ProgressRecords)
            .SingleOrDefaultAsync(e => e.Id == enrollmentId);

        if (enrollment == null)
        {
            return 0;
        }

        // Get the course
        var course = await _context.Courses
            .Include(c => c.Modules)
            .ThenInclude(m => m.Lessons)
            .SingleOrDefaultAsync(c => c.Id == enrollment.CourseId);

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

    public async Task AddAsync(Progress progress)
    {
        await _context.Progresses.AddAsync(progress);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Progress progress)
    {
        _context.Entry(progress).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
