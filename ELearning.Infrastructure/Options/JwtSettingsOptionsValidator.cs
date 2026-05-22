// <copyright file="JwtSettingsOptionsValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

using System.Text;
using Microsoft.Extensions.Options;

public sealed class JwtSettingsOptionsValidator : IValidateOptions<JwtSettingsOptions>
{
    private const int MinimumSecretBytes = 32;

    public ValidateOptionsResult Validate(string? name, JwtSettingsOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            failures.Add("JwtSettings:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("JwtSettings:Audience is required.");
        }

        if (Encoding.UTF8.GetByteCount(options.Secret) < MinimumSecretBytes)
        {
            failures.Add($"JwtSettings:Secret must be at least {MinimumSecretBytes} bytes.");
        }

        if (options.ExpiryInDays <= 0)
        {
            failures.Add("JwtSettings:ExpiryInDays must be greater than zero.");
        }

        if (options.RefreshTokenExpiryInDays <= 0)
        {
            failures.Add("JwtSettings:RefreshTokenExpiryInDays must be greater than zero.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
