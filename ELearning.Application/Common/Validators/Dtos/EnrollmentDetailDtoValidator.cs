// <copyright file="EnrollmentDetailDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Enrollments.Dtos;
using FluentValidation;

public sealed class EnrollmentDetailDtoValidator : AbstractValidator<EnrollmentDetailDto>
{
    public EnrollmentDetailDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.StudentId).NotEmpty();
        this.RuleFor(x => x.StudentName).NotEmpty();
        this.RuleFor(x => x.CourseId).NotEmpty();
        this.RuleFor(x => x.CourseTitle).NotEmpty();
        this.RuleFor(x => x.Status).NotEmpty();
        this.RuleFor(x => x.CompletionPercentage).InclusiveBetween(0, 100);
        this.RuleFor(x => x.LessonProgress).NotNull();
        this.RuleFor(x => x.Submissions).NotNull();
    }
}
