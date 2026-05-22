// <copyright file="JwtSettingsOptions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

public sealed class JwtSettingsOptions
{
    public const string SectionName = "JwtSettings";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string Secret { get; init; } = string.Empty;

    public double ExpiryInDays { get; init; }

    public int RefreshTokenExpiryInDays { get; init; } = 14;
}
