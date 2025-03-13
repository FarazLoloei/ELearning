using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;

namespace ELearning.Infrastructure.Dapr.Abstraction;

// Interface for Submission read service
public interface ISubmissionReadService : IReadService<SubmissionDetailDto, Guid>
{
    Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(Guid instructorId, int pageNumber, int pageSize);

    Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, int pageNumber, int pageSize);

    Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, int pageNumber, int pageSize);
}