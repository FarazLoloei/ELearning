// <copyright file="ObservabilityOptions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; init; } = "elearning-api";

    public bool ConsoleExporterEnabled { get; init; }

    public string? OtlpEndpoint { get; init; }
}
