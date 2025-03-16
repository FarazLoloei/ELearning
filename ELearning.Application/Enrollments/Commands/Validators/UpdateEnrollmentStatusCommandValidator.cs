using FluentValidation;

namespace ELearning.Application.Enrollments.Commands.Validators;

/// <summary>
/// Validator for UpdateEnrollmentStatusCommand
/// </summary>
public class UpdateEnrollmentStatusCommandValidator : AbstractValidator<UpdateEnrollmentStatusCommand>
{
    public UpdateEnrollmentStatusCommandValidator()
    {
        RuleFor(v => v.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => new[] { "Active", "Paused", "Completed", "Abandoned" }.Contains(status))
            .WithMessage("Status must be one of: Active, Paused, Completed, Abandoned");
    }
}