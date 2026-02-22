using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Submissions.Commands;

public sealed record CreateSubmissionCommand : IRequest<Result>
{
    public Guid AssignmentId { get; init; }

    /// <summary>
    /// Text content of the submission
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// URL to any uploaded file for the submission
    /// </summary>
    public string FileUrl { get; init; } = string.Empty;
}
