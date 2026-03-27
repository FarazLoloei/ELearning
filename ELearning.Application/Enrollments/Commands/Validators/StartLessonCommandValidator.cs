// <copyright file="StartLessonCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands.Validators;

using FluentValidation;

public sealed class StartLessonCommandValidator : AbstractValidator<StartLessonCommand>
{
    public StartLessonCommandValidator()
    {
        this.RuleFor(v => v.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        this.RuleFor(v => v.LessonId)
            .NotEmpty().WithMessage("Lesson ID is required.");
    }
}
