// <copyright file="AddCourseModuleCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class AddCourseModuleCommandValidator : AbstractValidator<AddCourseModuleCommand>
{
    public AddCourseModuleCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        this.RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        this.RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        this.RuleFor(v => v.Order)
            .GreaterThan(0).WithMessage("Order must be greater than 0.");
    }
}
