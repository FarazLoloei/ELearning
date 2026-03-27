// <copyright file="RefreshAuthTokenCommandValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Auth.Commands.Validators;

using FluentValidation;

public sealed class RefreshAuthTokenCommandValidator : AbstractValidator<RefreshAuthTokenCommand>
{
    public RefreshAuthTokenCommandValidator()
    {
        this.RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}
