using FluentValidation;

namespace ELearning.Application.Submissions.Commands.Validators;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
    {
        RuleFor(v => v.AssignmentId)
            .NotEmpty().WithMessage("Assignment ID is required.");

        RuleFor(v => v.Content)
            .NotEmpty().When(cmd => string.IsNullOrEmpty(cmd.FileUrl))
            .WithMessage("Either content or file must be provided.");

        RuleFor(v => v.FileUrl)
            .NotEmpty().When(cmd => string.IsNullOrEmpty(cmd.Content))
            .WithMessage("Either content or file must be provided.");
    }
}