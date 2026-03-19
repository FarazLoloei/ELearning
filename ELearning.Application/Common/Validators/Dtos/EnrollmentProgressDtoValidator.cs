// <copyright file="EnrollmentProgressDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Students.Dtos;
using FluentValidation;

public sealed class EnrollmentProgressDtoValidator : AbstractValidator<EnrollmentProgressDto>
{
    public EnrollmentProgressDtoValidator()
    {
        this.RuleFor(x => x.EnrollmentId).NotEmpty();
        this.RuleFor(x => x.CourseId).NotEmpty();
        this.RuleFor(x => x.CourseTitle).NotEmpty();
        this.RuleFor(x => x.Status).NotEmpty();
        this.RuleFor(x => x.CompletionPercentage).InclusiveBetween(0, 100);
        this.RuleFor(x => x.CompletedLessons).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.TotalLessons).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.CompletedAssignments).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.TotalAssignments).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.CompletedLessons).LessThanOrEqualTo(x => x.TotalLessons);
        this.RuleFor(x => x.CompletedAssignments).LessThanOrEqualTo(x => x.TotalAssignments);
    }
}
