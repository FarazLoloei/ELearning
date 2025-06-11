using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Submissions.Abstractions.ReadModels;

/// <summary>
/// Provides read-only access to submission-related data.
/// </summary>
public interface ISubmissionReadService : IReadRepository<SubmissionDetailDto, Guid>
{
    /// <summary>
    /// Retrieves a paginated list of pending submissions for a specific instructor.
    /// </summary>
    /// <param name="instructorId">The unique identifier of the instructor.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(
        Guid instructorId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of submissions made by a specific student.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(
        Guid studentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated list of submissions for a specific assignment.
    /// </summary>
    /// <param name="assignmentId">The unique identifier of the assignment.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(
        Guid assignmentId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}