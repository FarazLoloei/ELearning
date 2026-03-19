// <copyright file="AssignmentDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class AssignmentDtoValidator : AbstractValidator<AssignmentDto>
{
    public AssignmentDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Description).NotEmpty();
        this.RuleFor(x => x.Type).NotEmpty();
        this.RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
    }
}
