using ELearning.Application.Enrollments.ReadModels;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Abstractions;

public interface IProgressReadRepository : IReadRepository<ProgressReadModel, Guid>
{
    Task<IReadOnlyList<ProgressReadModel>> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<ProgressReadModel?> GetByEnrollmentAndLessonIdAsync(Guid enrollmentId, Guid lessonId, CancellationToken cancellationToken = default);

    Task<double> GetCourseProgressPercentageAsync(Guid enrollmentId, CancellationToken cancellationToken = default);
}