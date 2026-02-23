using ELearning.Domain.Entities.CourseAggregate;
using Microsoft.EntityFrameworkCore;

namespace ELearning.Infrastructure.Data.Repositories;

public class AssignmentReadRepository(ApplicationDbContext context)
{
    public async Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken)
    {
        return await context.Assignments
            .AsNoTracking()
            .Where(a => a.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await context.Assignments
            .AsNoTracking()
            .Join(context.Modules,
                a => a.ModuleId,
                m => m.Id,
                (a, m) => new { Assignment = a, Module = m })
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => x.Assignment)
            .ToListAsync(cancellationToken);
    }
}