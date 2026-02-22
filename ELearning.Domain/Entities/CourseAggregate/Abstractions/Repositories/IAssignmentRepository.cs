namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Assignment>> GetByModuleIdAsync(Guid moduleId);

    Task<IReadOnlyList<Assignment>> GetByCourseIdAsync(Guid courseId);

    Task<Module?> GetModuleForAssignmentAsync(Guid assignmentId);

    Task AddAsync(Assignment assignment);

    Task UpdateAsync(Assignment assignment);

    Task DeleteAsync(Assignment assignment);
}
