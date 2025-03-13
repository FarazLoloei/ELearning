using FluentValidation;

namespace ELearning.Application.Enrollments.Commands.Validators;

public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}