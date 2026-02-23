namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);

    Task AddAsync(Lesson lesson, CancellationToken cancellationToken = default);

    Task UpdateAsync(Lesson lesson, CancellationToken cancellationToken = default);

    Task DeleteAsync(Lesson lesson, CancellationToken cancellationToken = default);
}
