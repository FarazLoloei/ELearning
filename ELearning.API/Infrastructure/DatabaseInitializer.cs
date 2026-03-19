// <copyright file="DatabaseInitializer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using ELearning.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public static class DatabaseInitializer
{
    private const string SqlServerProvider = "SqlServer";

    public static bool ShouldApplyMigrations(string? provider) =>
        string.Equals(provider, SqlServerProvider, StringComparison.OrdinalIgnoreCase);

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var provider = configuration["Database:Provider"];

        if (ShouldApplyMigrations(provider))
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            return;
        }

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
