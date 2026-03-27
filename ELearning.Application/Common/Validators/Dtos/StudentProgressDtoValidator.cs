// <copyright file="StudentProgressDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Students.Dtos;
using FluentValidation;

public sealed class StudentProgressDtoValidator : AbstractValidator<StudentProgressDto>
{
    public StudentProgressDtoValidator()
    {
        this.RuleFor(x => x.StudentId).NotEmpty();
        this.RuleFor(x => x.StudentName).NotEmpty();
        this.RuleFor(x => x.CompletedCourses).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.InProgressCourses).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Enrollments).NotNull();
    }
}
