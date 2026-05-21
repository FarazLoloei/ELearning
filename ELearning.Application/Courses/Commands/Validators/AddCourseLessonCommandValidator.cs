// <copyright file="AddCourseLessonCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class AddCourseLessonCommandValidator : AbstractValidator<AddCourseLessonCommand>
{
    public AddCourseLessonCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        this.RuleFor(v => v.ModuleId)
            .NotEmpty().WithMessage("Module ID is required.");

        this.RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        this.RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.");

        this.RuleFor(v => v.TypeId)
            .GreaterThan(0).WithMessage("Lesson type is required.");

        this.RuleFor(v => v.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");

        this.RuleFor(v => v.DurationHours)
            .GreaterThanOrEqualTo(0).WithMessage("Duration hours must be greater than or equal to 0.");

        this.RuleFor(v => v.DurationMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Duration minutes must be greater than or equal to 0.")
            .LessThan(60).WithMessage("Duration minutes must be less than 60.");
    }
}
