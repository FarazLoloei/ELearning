using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface IEnrollmentRepository : IEntityFrameworkRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> GetRecentEnrollmentsAsync(int count, CancellationToken cancellationToken);

    Task<int> GetEnrollmentsCountAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<Enrollment>> GetCompletedEnrollmentsAsync(Guid studentId, CancellationToken cancellationToken);
}