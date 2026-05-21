// <copyright file="AddCourseAssignmentCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class AddCourseAssignmentCommandValidator : AbstractValidator<AddCourseAssignmentCommand>
{
    public AddCourseAssignmentCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        this.RuleFor(v => v.ModuleId)
            .NotEmpty().WithMessage("Module ID is required.");

        this.RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        this.RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.");

        this.RuleFor(v => v.TypeId)
            .GreaterThan(0).WithMessage("Assignment type is required.");

        this.RuleFor(v => v.MaxPoints)
            .GreaterThan(0).WithMessage("Maximum points must be greater than 0.");
    }
}
