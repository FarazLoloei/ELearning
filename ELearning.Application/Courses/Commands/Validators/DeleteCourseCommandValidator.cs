using FluentValidation;

namespace ELearning.Application.Courses.Commands.Validators;

/// <summary>
/// Validator for DeleteCourseCommand
/// </summary>
public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}