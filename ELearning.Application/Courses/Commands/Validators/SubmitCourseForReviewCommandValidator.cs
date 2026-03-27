// <copyright file="SubmitCourseForReviewCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class SubmitCourseForReviewCommandValidator : AbstractValidator<SubmitCourseForReviewCommand>
{
    public SubmitCourseForReviewCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");
    }
}
