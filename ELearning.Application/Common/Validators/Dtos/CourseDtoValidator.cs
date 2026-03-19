// <copyright file="CourseDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class CourseDtoValidator : AbstractValidator<CourseDto>
{
    public CourseDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Description).NotEmpty();
        this.RuleFor(x => x.Instructor).NotNull();
        this.RuleFor(x => x.Status).NotEmpty();
        this.RuleFor(x => x.Category).NotEmpty();
        this.RuleFor(x => x.Level).NotEmpty();
        this.RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        this.RuleFor(x => x.NumberOfRatings).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Modules).NotNull();
        this.RuleFor(x => x.Reviews).NotNull();
    }
}
