using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Submissions.Abstractions.ReadModels;

public interface ISubmissionReadService : IReadRepository<SubmissionDetailDto, Guid>
{
    Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(Guid instructorId, int pageNumber, int pageSize);

    Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, int pageNumber, int pageSize);

    Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, int pageNumber, int pageSize);
}