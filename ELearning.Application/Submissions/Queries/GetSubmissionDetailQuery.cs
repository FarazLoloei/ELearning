using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using MediatR;

namespace ELearning.Application.Submissions.Queries;

/// <summary>
/// Query to get details of a submission
/// </summary>
public record GetSubmissionDetailQuery : IRequest<Result<SubmissionDetailDto>>
{
    public Guid SubmissionId { get; set; }
}