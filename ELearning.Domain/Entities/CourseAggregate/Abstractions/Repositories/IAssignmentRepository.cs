namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    Task AddAsync(Assignment assignment, CancellationToken cancellationToken = default);

    Task UpdateAsync(Assignment assignment, CancellationToken cancellationToken = default);

    Task DeleteAsync(Assignment assignment, CancellationToken cancellationToken = default);
}
