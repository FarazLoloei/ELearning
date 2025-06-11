using ELearning.Application.Enrollments.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Abstractions.ReadModels;

/// <summary>
/// Provides read-only access to enrollment-related data.
/// </summary>
public interface IEnrollmentReadService : IReadRepository<EnrollmentDetailDto, Guid>
{
    /// <summary>
    /// Retrieves a paginated list of enrollments for a specific student.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(
        Guid studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of enrollments for a specific course.
    /// </summary>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(
        Guid courseId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific enrollment by student and course identifiers.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <param name="courseId">The unique identifier of the course.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(
        Guid studentId,
        Guid courseId,
        CancellationToken cancellationToken = default);
}