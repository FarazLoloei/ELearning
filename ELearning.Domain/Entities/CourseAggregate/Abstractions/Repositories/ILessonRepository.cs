namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Lesson>> GetByModuleIdForUpdateAsync(Guid moduleId, CancellationToken cancellationToken = default);
}
