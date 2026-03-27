// <copyright file="ReviewCourseCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands.Validators;

using FluentValidation;

public sealed class ReviewCourseCommandValidator : AbstractValidator<ReviewCourseCommand>
{
    public ReviewCourseCommandValidator()
    {
        this.RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("EnrollmentId is required.");

        this.RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        this.RuleFor(x => x.Review)
            .MaximumLength(1000).WithMessage("Review must not exceed 1000 characters.");
    }
}
