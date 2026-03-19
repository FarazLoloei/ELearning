// <copyright file="InstructorCourseDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Instructors.Dtos;
using FluentValidation;

public sealed class InstructorCourseDtoValidator : AbstractValidator<InstructorCourseDto>
{
    public InstructorCourseDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Category).NotEmpty();
        this.RuleFor(x => x.EnrollmentsCount).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Status).NotEmpty();
    }
}
