// <copyright file="DatabaseProviderNames.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

public static class DatabaseProviderNames
{
    public const string SqliteInMemory = "SqliteInMemory";

    public const string SqlServer = "SqlServer";

    public static bool IsSqliteInMemory(string? provider) =>
        string.Equals(provider, SqliteInMemory, StringComparison.OrdinalIgnoreCase);

    public static bool IsSqlServer(string? provider) =>
        string.Equals(provider, SqlServer, StringComparison.OrdinalIgnoreCase);

    public static bool IsSupported(string? provider) =>
        IsSqliteInMemory(provider) || IsSqlServer(provider);
}
