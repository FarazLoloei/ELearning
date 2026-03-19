// <copyright file="CreateCourseCommandValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        this.RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        this.RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        this.RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        this.RuleFor(v => v.LevelId)
            .NotEmpty().WithMessage("Level is required.");

        this.RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        this.RuleFor(v => v.DurationHours)
            .GreaterThanOrEqualTo(0).WithMessage("Duration hours must be greater than or equal to 0.");

        this.RuleFor(v => v.DurationMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("Duration minutes must be greater than or equal to 0.")
            .LessThan(60).WithMessage("Duration minutes must be less than 60.");
    }
}