// <copyright file="IAssignmentReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IAssignmentReadRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
}
