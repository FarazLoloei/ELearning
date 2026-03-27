// <copyright file="OutboxDispatcherIntegrationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Application;
using ELearning.Infrastructure;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Outbox;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class OutboxDispatcherIntegrationTests
{
    [Fact]
    public async Task DispatchPendingAsync_ShouldMarkMessagesAsProcessed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared",
                ["JwtSettings:Issuer"] = "integration-tests",
                ["JwtSettings:Audience"] = "integration-tests",
                ["JwtSettings:Secret"] = "integration-tests-secret-key-with-32chars",
                ["JwtSettings:ExpiryInDays"] = "7",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();

        using (var seedScope = provider.CreateScope())
        {
            var dbContext = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Type = "test.event",
                Payload = "{\"value\":1}",
                OccurredOnUtc = DateTime.UtcNow,
            });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        using (var dispatchScope = provider.CreateScope())
        {
            var dispatcher = dispatchScope.ServiceProvider.GetRequiredService<IOutboxDispatcher>();
            var dispatchedCount = await dispatcher.DispatchPendingAsync(cancellationToken);
            dispatchedCount.Should().Be(1);
        }

        using (var assertScope = provider.CreateScope())
        {
            var dbContext = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var message = dbContext.OutboxMessages.Single();
            message.ProcessedOnUtc.Should().NotBeNull();
            message.Error.Should().BeNull();
        }
    }
}
