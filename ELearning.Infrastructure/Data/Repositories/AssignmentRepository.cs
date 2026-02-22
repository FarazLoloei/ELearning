using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

// Assignment Repository Implementation
public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id)
    {
        return await _context.Assignments
            .SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId)
    {
        return await _context.Assignments
            .Where(a => a.ModuleId == moduleId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId)
    {
        return await _context.Assignments
            .Join(_context.Modules,
                a => a.ModuleId,
                m => m.Id,
                (a, m) => new { Assignment = a, Module = m })
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => x.Assignment)
            .ToListAsync();
    }

    public async Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId)
    {
        var assignment = await _context.Assignments
            .FindAsync(assignmentId);

        if (assignment == null)
            return null;

        return await _context.Modules
            .FindAsync(assignment.ModuleId);
    }

    public async Task AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        _context.Entry(assignment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Assignment assignment)
    {
        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
    }
}