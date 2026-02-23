using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;

namespace ELearning.Application.Enrollments.Abstractions.ReadModels;

public interface IEnrollmentReadRepository
{
    Task<PaginatedList<Enrollment>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<int> GetCourseEnrollmentCountAsync(
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetCourseEnrollmentCountsAsync(
        IReadOnlyCollection<Guid> courseIds,
        CancellationToken cancellationToken = default);
}
