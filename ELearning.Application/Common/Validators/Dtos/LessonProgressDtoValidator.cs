// <copyright file="LessonProgressDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Enrollments.Dtos;
using FluentValidation;

public sealed class LessonProgressDtoValidator : AbstractValidator<LessonProgressDto>
{
    public LessonProgressDtoValidator()
    {
        this.RuleFor(x => x.LessonId).NotEmpty();
        this.RuleFor(x => x.LessonTitle).NotEmpty();
        this.RuleFor(x => x.Status).NotEmpty();
        this.RuleFor(x => x.TimeSpentSeconds).GreaterThanOrEqualTo(0);
    }
}
