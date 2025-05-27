using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using ELearning.SharedKernel;
using MediatR;

namespace ELearning.Application.Submissions.Queries;

/// <summary>
/// Query to get pending submissions for an instructor
/// </summary>
public record GetPendingSubmissionsQuery : IRequest<Result<PaginatedList<SubmissionDto>>>
{
    public Guid InstructorId { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}