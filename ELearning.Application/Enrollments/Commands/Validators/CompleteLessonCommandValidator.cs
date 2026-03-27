// <copyright file="CompleteLessonCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands.Validators;

using FluentValidation;

public sealed class CompleteLessonCommandValidator : AbstractValidator<CompleteLessonCommand>
{
    public CompleteLessonCommandValidator()
    {
        this.RuleFor(v => v.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        this.RuleFor(v => v.LessonId)
            .NotEmpty().WithMessage("Lesson ID is required.");
    }
}
