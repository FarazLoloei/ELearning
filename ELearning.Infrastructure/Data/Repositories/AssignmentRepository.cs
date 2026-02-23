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

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Where(a => a.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _context.Assignments
            .Join(_context.Modules,
                a => a.ModuleId,
                m => m.Id,
                (a, m) => new { Assignment = a, Module = m })
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => x.Assignment)
            .ToListAsync(cancellationToken);
    }

    public async Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await _context.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignment == null)
            return null;

        return await _context.Modules
            .FindAsync([assignment.ModuleId], cancellationToken);
    }

    public async Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        await _context.Assignments.AddAsync(assignment, cancellationToken);
    }

    public Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Entry(assignment).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default)
    {
        _context.Assignments.Remove(assignment);
        return Task.CompletedTask;
    }
}
