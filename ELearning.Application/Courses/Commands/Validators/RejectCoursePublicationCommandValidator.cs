// <copyright file="RejectCoursePublicationCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Commands.Validators;

using FluentValidation;

public sealed class RejectCoursePublicationCommandValidator : AbstractValidator<RejectCoursePublicationCommand>
{
    public RejectCoursePublicationCommandValidator()
    {
        this.RuleFor(v => v.CourseId)
            .NotEmpty().WithMessage("Course ID is required.");

        this.RuleFor(v => v.Reason)
            .NotEmpty().WithMessage("A rejection reason is required.")
            .MaximumLength(1000).WithMessage("The rejection reason must not exceed 1000 characters.");
    }
}
