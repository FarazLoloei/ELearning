// <copyright file="AssignmentReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

// Assignment Repository Implementation
public class AssignmentReadRepository : IAssignmentReadRepository
{
    private readonly ApplicationDbContext context;

    public AssignmentReadRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await this.context.Assignments
            .SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        return await this.context.Assignments
            .Where(a => a.ModuleId == moduleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await this.context.Assignments
            .Join(
                this.context.Modules,
                a => a.ModuleId,
                m => m.Id,
                (a, m) => new { Assignment = a, Module = m })
            .Where(x => x.Module.CourseId == courseId)
            .Select(x => x.Assignment)
            .ToListAsync(cancellationToken);
    }

    public async Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await this.context.Assignments
            .FindAsync([assignmentId], cancellationToken);

        if (assignment == null)
        {
            return null;
        }

        return await this.context.Modules
            .FindAsync([assignment.ModuleId], cancellationToken);
    }
}
