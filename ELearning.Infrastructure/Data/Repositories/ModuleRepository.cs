using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Module Repository Implementation
public class ModuleRepository : IModuleRepository
{
    private readonly ApplicationDbContext _context;

    public ModuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Modules
            .Include(m => m.Lessons)
            .Include(m => m.Assignments)
            .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Modules
            .Include(m => m.Lessons)
            .Include(m => m.Assignments)
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Order)
            .ToListAsync(cancellationToken);
    }

}
