// <copyright file="StudentDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Students.Dtos;
using FluentValidation;

public sealed class StudentDtoValidator : AbstractValidator<StudentDto>
{
    public StudentDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.FullName).NotEmpty();
        this.RuleFor(x => x.Email).NotEmpty();
        this.RuleFor(x => x.ProfilePictureUrl).NotEmpty();
    }
}
