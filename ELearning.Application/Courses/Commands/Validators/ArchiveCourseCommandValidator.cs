// <copyright file="ArchiveCourseCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class ArchiveCourseCommandValidator : AbstractValidator<ArchiveCourseCommand>
{
    public ArchiveCourseCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}
