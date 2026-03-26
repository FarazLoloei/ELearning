// <copyright file="SubmissionDtoValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Validators.Dtos;

using ELearning.Application.Submissions.Dtos;
using FluentValidation;

public sealed class SubmissionDtoValidator : AbstractValidator<SubmissionDto>
{
    public SubmissionDtoValidator()
    {
        this.RuleFor(x => x.Id).NotEmpty();
        this.RuleFor(x => x.AssignmentId).NotEmpty();
        this.RuleFor(x => x.AssignmentTitle).NotEmpty();
        this.RuleFor(x => x.MaxPoints).GreaterThanOrEqualTo(0);
        this.RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
        this.RuleFor(x => x.Score).LessThanOrEqualTo(x => x.MaxPoints).When(x => x.Score.HasValue);
    }
}
