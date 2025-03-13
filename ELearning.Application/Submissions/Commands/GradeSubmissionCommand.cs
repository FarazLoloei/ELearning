using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Submissions.Commands;

public class GradeSubmissionCommand : IRequest<Result>
{
    public Guid SubmissionId { get; set; }

    public int Score { get; set; }

    public string Feedback { get; set; }
}