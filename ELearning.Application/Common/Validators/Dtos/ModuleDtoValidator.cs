// <copyright file="ModuleDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Courses.Dtos;
using FluentValidation;

public sealed class ModuleDtoValidator : AbstractValidator<ModuleDto>
{
    public ModuleDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.Title).NotEmpty();
        this.RuleFor(x => x.Description).NotEmpty();
        this.RuleFor(x => x.Order).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Lessons).NotNull();
        this.RuleFor(x => x.Assignments).NotNull();
    }
}
