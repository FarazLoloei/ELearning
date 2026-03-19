// <copyright file="InstructorDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Instructors.Dtos;
using FluentValidation;

public sealed class InstructorDtoValidator : AbstractValidator<InstructorDto>
{
    public InstructorDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.FullName).NotEmpty();
        this.RuleFor(x => x.Email).NotEmpty();
        this.RuleFor(x => x.Bio).NotEmpty();
        this.RuleFor(x => x.Expertise).NotEmpty();
        this.RuleFor(x => x.ProfilePictureUrl).NotEmpty();
        this.RuleFor(x => x.AverageRating).InclusiveBetween(0, 5);
        this.RuleFor(x => x.TotalStudents).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.TotalCourses).GreaterThanOrEqualTo(0);
    }
}
