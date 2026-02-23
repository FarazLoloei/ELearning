namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface IProgressRepository
{
    Task<Progress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

    Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task AddAsync(Progress progress, CancellationToken cancellationToken = default);

    Task UpdateAsync(Progress progress, CancellationToken cancellationToken = default);
}
