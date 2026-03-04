using ELearning.Domain.Entities.EnrollmentAggregate;

namespace ELearning.Application.Enrollments.Abstractions.ReadModels;

public interface IProgressReadRepository
{
    Task<Progress?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Progress>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<Progress?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

    Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}
