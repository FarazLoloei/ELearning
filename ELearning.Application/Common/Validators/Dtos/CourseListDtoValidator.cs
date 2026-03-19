// <copyright file="CourseListDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class CourseListDtoValidator : AbstractValidator<CourseListDto>
{
    public CourseListDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Description).NotEmpty();
        this.RuleFor(x => x.InstructorName).NotEmpty();
        this.RuleFor(x => x.Category).NotEmpty();
        this.RuleFor(x => x.Level).NotEmpty();
        this.RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        this.RuleFor(x => x.NumberOfRatings).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.EnrollmentsCount).GreaterThanOrEqualTo(0);
    }
}
