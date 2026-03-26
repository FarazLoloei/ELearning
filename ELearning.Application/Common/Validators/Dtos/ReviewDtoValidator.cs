// <copyright file="ReviewDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class ReviewDtoValidator : AbstractValidator<ReviewDto>
{
    public ReviewDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.StudentName).NotEmpty();
        this.RuleFor(x => x.Rating).InclusiveBetween(0, 5);
        this.RuleFor(x => x.Comment).NotEmpty();
    }
}
