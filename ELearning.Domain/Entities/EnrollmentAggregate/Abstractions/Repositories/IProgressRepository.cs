namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface IProgressRepository
{
    Task<Progress?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId);

    Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId);

    Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId);

    Task AddAsync(Progress progress);

    Task UpdateAsync(Progress progress);
}
