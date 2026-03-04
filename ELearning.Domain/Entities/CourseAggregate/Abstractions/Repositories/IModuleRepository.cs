namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Module>> GetByCourseIdForUpdateAsync(Guid courseId, CancellationToken cancellationToken = default);
}
