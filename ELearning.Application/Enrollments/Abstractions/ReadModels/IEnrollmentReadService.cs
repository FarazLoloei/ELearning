using ELearning.Application.Enrollments.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Abstractions.ReadModels;

public interface IEnrollmentReadService : IReadRepository<EnrollmentDetailDto, Guid>
{
    Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, int pageNumber, int pageSize);

    Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, int pageNumber, int pageSize);

    Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId);
}