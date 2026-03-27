// <copyright file="UpdateEnrollmentStatusCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Commands.Validators;

using FluentValidation;

/// <summary>
/// Validator for UpdateEnrollmentStatusCommand.
/// </summary>
public class UpdateEnrollmentStatusCommandValidator : AbstractValidator<UpdateEnrollmentStatusCommand>
{
    public UpdateEnrollmentStatusCommandValidator()
    {
        this.RuleFor(v => v.EnrollmentId)
            .NotEmpty().WithMessage("Enrollment ID is required.");

        this.RuleFor(v => v.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(status => new[] { "Active", "Paused", "Abandoned" }.Contains(status, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Status must be one of: Active, Paused, Abandoned");
    }
}
