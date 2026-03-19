// <copyright file="LessonDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class LessonDtoValidator : AbstractValidator<LessonDto>
{
    public LessonDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Content).NotEmpty();
        this.RuleFor(x => x.Type).NotEmpty();
        this.RuleFor(x => x.VideoUrl).NotEmpty();
        this.RuleFor(x => x.Duration).NotEmpty();
        this.RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
    }
}
