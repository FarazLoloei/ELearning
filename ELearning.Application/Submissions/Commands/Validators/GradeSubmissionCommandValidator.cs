using FluentValidation;

namespace ELearning.Application.Submissions.Commands.Validators;

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionCommandValidator()
    {
        RuleFor(v => v.SubmissionId)
            .NotEmpty().WithMessage("Submission ID is required.");

        RuleFor(v => v.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0.");

        RuleFor(v => v.Feedback)
            .NotEmpty().WithMessage("Feedback is required.");
    }
}