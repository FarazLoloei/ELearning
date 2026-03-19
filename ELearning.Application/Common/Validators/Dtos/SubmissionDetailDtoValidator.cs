// <copyright file="SubmissionDetailDtoValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Submissions.Dtos;
using FluentValidation;

public sealed class SubmissionDetailDtoValidator : AbstractValidator<SubmissionDetailDto>
{
    public SubmissionDetailDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.AssignmentId).NotEmpty();
        this.RuleFor(x => x.AssignmentTitle).NotEmpty();
        this.RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        this.RuleFor(x => x.Score).LessThanOrEqualTo(x => x.MaxPoints).When(x => x.Score.HasValue);
        this.RuleFor(x => x.StudentId).NotEmpty();
        this.RuleFor(x => x.StudentName).NotEmpty();
        this.RuleFor(x => x.Content).NotEmpty();
        this.RuleFor(x => x.FileUrl).NotEmpty();
        this.RuleFor(x => x.Feedback).NotEmpty();
        this.RuleFor(x => x.GradedByName).NotEmpty();
    }
}
