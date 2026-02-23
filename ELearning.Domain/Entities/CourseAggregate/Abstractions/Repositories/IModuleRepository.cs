namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
}
