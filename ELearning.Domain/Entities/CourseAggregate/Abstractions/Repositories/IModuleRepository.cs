namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);

    Task AddAsync(Module module, CancellationToken cancellationToken = default);

    Task UpdateAsync(Module module, CancellationToken cancellationToken = default);

    Task DeleteAsync(Module module, CancellationToken cancellationToken = default);
}
