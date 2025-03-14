using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Submissions.Commands;

public class CreateSubmissionCommand : IRequest<Result>
{
    public Guid AssignmentId { get; set; }

    /// <summary>
    /// Text content of the submission
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// URL to any uploaded file for the submission
    /// </summary>
    public string FileUrl { get; set; }
}