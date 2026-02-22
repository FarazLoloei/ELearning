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

    public async Task<Module?> GetByIdAsync(Guid id)
    {
        return await _context.Modules
            .Include(m => m.Lessons)
            .Include(m => m.Assignments)
            .SingleOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Modules
            .Include(m => m.Lessons)
            .Include(m => m.Assignments)
            .Where(m => m.CourseId == courseId)
            .OrderBy(m => m.Order)
            .ToListAsync();
    }

    public async Task AddAsync(Module module)
    {
        await _context.Modules.AddAsync(module);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Module module)
    {
        _context.Entry(module).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Module module)
    {
        _context.Modules.Remove(module);
        await _context.SaveChangesAsync();
    }
}
