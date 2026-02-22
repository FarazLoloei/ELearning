using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Submissions.Commands;

public sealed record GradeSubmissionCommand : IRequest<Result>
{
    public Guid SubmissionId { get; init; }

    public int Score { get; init; }

    public string Feedback { get; init; } = string.Empty;
}
