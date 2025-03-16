using FluentValidation;

namespace ELearning.Application.Courses.Commands.Validators;

/// <summary>
/// Validator for UpdateCourseCommand
/// </summary>
public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(v => v.LevelId)
            .NotEmpty().WithMessage("Level is required.");

        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(v => v.DurationHours)
            .GreaterThanOrEqualTo(0).WithMessage("Duration hours must be greater than or equal to 0.");

        RuleFor(v => v.DurationMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Duration minutes must be greater than or equal to 0.")
            .LessThan(60).WithMessage("Duration minutes must be less than 60.");
    }
}