// <copyright file="DatabaseOptionsValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

public sealed class DatabaseOptionsValidator(IConfiguration configuration) : IValidateOptions<DatabaseOptions>
{
    public ValidateOptionsResult Validate(string? name, DatabaseOptions options)
    {
        if (!DatabaseProviderNames.IsSupported(options.Provider))
        {
            return ValidateOptionsResult.Fail(
                $"Database:Provider must be '{DatabaseProviderNames.SqliteInMemory}' or '{DatabaseProviderNames.SqlServer}'.");
        }

        if (DatabaseProviderNames.IsSqliteInMemory(options.Provider) &&
            string.IsNullOrWhiteSpace(options.SqliteInMemoryConnection))
        {
            return ValidateOptionsResult.Fail("Database:SqliteInMemoryConnection is required when using SqliteInMemory.");
        }

        if (DatabaseProviderNames.IsSqlServer(options.Provider) &&
            string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
        {
            return ValidateOptionsResult.Fail("ConnectionStrings:DefaultConnection is required when using SqlServer.");
        }

        return ValidateOptionsResult.Success;
    }
}
