using ELearning.Application.Submissions.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class SubmissionDetailDtoValidator : AbstractValidator<SubmissionDetailDto>
{
    public SubmissionDetailDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x.AssignmentTitle).NotEmpty();
        RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        RuleFor(x => x.Score).LessThanOrEqualTo(x => x.MaxPoints).When(x => x.Score.HasValue);
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.StudentName).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.FileUrl).NotEmpty();
        RuleFor(x => x.Feedback).NotEmpty();
        RuleFor(x => x.GradedByName).NotEmpty();
    }
}
