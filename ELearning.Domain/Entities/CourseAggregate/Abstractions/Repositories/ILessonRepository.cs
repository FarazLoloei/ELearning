namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface ILessonRepository
{
    Task<Lesson> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Lesson>> GetByModuleIdAsync(Guid moduleId);

    Task AddAsync(Lesson lesson);

    Task UpdateAsync(Lesson lesson);

    Task DeleteAsync(Lesson lesson);
}