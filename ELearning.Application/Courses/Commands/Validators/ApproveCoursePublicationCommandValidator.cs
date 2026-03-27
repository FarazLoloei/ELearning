// <copyright file="ApproveCoursePublicationCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class ApproveCoursePublicationCommandValidator : AbstractValidator<ApproveCoursePublicationCommand>
{
    public ApproveCoursePublicationCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}
