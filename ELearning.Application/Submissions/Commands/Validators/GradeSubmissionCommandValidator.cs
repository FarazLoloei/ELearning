// <copyright file="GradeSubmissionCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Commands.Validators;

using FluentValidation;

public class GradeSubmissionCommandValidator : AbstractValidator<GradeSubmissionCommand>
{
    public GradeSubmissionCommandValidator()
    {
        this.RuleFor(v => v.SubmissionId)
            .NotEmpty().WithMessage("Submission ID is required.");

        this.RuleFor(v => v.Score)
            .GreaterThanOrEqualTo(0).WithMessage("Score must be greater than or equal to 0.");

        this.RuleFor(v => v.Feedback)
            .NotEmpty().WithMessage("Feedback is required.");
    }
}