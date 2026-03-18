using ELearning.Application.Enrollments.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

namespace ELearning.Application.Enrollments.Abstractions;

public interface IEnrollmentReadRepository : IReadRepository<EnrollmentDetailReadModel, Guid>
{
    Task<EnrollmentDetailReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<EnrollmentSummaryReadModel>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);
}