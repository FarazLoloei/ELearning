using ELearning.Application.Submissions.Dtos;
using FluentValidation;

namespace ELearning.Application.Common.Validators.Dtos;

public sealed class SubmissionDtoValidator : AbstractValidator<SubmissionDto>
{
    public SubmissionDtoValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.AssignmentId).NotEmpty();
        RuleFor(x => x.AssignmentTitle).NotEmpty();
        RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        RuleFor(x => x.Score).LessThanOrEqualTo(x => x.MaxPoints).When(x => x.Score.HasValue);
    }
}
