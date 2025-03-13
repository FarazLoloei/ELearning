using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Dtos;

namespace ELearning.Infrastructure.Dapr.Abstraction;

// Interface for Enrollment read service
public interface IEnrollmentReadService : IReadService<EnrollmentDetailDto, Guid>
{
    Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, int pageNumber, int pageSize);

    Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, int pageNumber, int pageSize);

    Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId);
}