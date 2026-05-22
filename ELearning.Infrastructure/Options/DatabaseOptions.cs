// <copyright file="DatabaseOptions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; init; } = DatabaseProviderNames.SqliteInMemory;

    public string SqliteInMemoryConnection { get; init; } = "Data Source=:memory:;Cache=Shared";
}
