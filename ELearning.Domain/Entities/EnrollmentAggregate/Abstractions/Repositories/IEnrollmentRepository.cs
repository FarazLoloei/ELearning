using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface IEnrollmentRepository : IEntityFrameworkRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);

    Task<Enrollment?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken);

    Task<bool> HasAnyForCourseAsync(Guid courseId, CancellationToken cancellationToken);
}
