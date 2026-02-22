namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface IModuleRepository
{
    Task<Module?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Module>> GetByCourseIdAsync(Guid courseId);

    Task AddAsync(Module module);

    Task UpdateAsync(Module module);

    Task DeleteAsync(Module module);
}
