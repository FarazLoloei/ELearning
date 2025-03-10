using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId);

    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId);

    Task<IReadOnlyList<Enrollment>> GetByCourseIdAsync(Guid courseId);

    Task<IReadOnlyList<Enrollment>> GetRecentEnrollmentsAsync(int count);

    Task<int> GetEnrollmentsCountAsync();

    Task<IReadOnlyList<Enrollment>> GetCompletedEnrollmentsAsync(Guid studentId);
}