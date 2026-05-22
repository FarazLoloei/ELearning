// <copyright file="DatabaseInitializer.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;

public static class DatabaseInitializer
{
    public static bool ShouldApplyMigrations(string? provider) =>
        DatabaseProviderNames.IsSqlServer(provider);

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration configuration, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var provider = configuration["Database:Provider"] ?? DatabaseProviderNames.SqliteInMemory;

        if (ShouldApplyMigrations(provider))
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            return;
        }

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
    }
}
