using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Lesson Repository Implementation
public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .SingleOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Lessons
            .Where(l => l.ModuleId == moduleId)
            .OrderBy(l => l.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        await _context.Lessons.AddAsync(lesson, cancellationToken);
    }

    public Task UpdateAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        _context.Entry(lesson).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Lesson lesson, CancellationToken cancellationToken = default)
    {
        _context.Lessons.Remove(lesson);
        return Task.CompletedTask;
    }
}
