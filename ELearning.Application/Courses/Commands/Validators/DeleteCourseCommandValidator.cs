// <copyright file="DeleteCourseCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

/// <summary>
/// Validator for DeleteCourseCommand.
/// </summary>
public class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
{
    public DeleteCourseCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}