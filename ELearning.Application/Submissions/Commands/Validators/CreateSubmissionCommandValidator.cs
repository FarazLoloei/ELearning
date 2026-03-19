// <copyright file="CreateSubmissionCommandValidator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Commands.Validators;

using FluentValidation;

public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
{
    public CreateSubmissionCommandValidator()
    {
        this.RuleFor(v => v.AssignmentId)
            .NotEmpty().WithMessage("Assignment ID is required.");

        this.RuleFor(v => v.Content)
            .NotEmpty().When(cmd => string.IsNullOrEmpty(cmd.FileUrl))
            .WithMessage("Either content or file must be provided.");

        this.RuleFor(v => v.FileUrl)
            .NotEmpty().When(cmd => string.IsNullOrEmpty(cmd.Content))
            .WithMessage("Either content or file must be provided.");
    }
}