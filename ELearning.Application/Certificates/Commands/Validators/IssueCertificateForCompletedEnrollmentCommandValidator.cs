// <copyright file="IssueCertificateForCompletedEnrollmentCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Commands.Validators;

using FluentValidation;

public sealed class IssueCertificateForCompletedEnrollmentCommandValidator : AbstractValidator<IssueCertificateForCompletedEnrollmentCommand>
{
    public IssueCertificateForCompletedEnrollmentCommandValidator()
    {
        this.RuleFor(x => x.EnrollmentId)
            .NotEmpty().WithMessage("EnrollmentId is required.");
    }
}
