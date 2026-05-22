// <copyright file="OutboxDispatcherIntegrationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Application;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Infrastructure;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Outbox;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public sealed class OutboxDispatcherIntegrationTests
{
    [Fact]
    public async Task DispatchPendingAsync_ShouldPublishMappedMessageAndMarkItProcessed()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var publisher = new FakeIntegrationEventPublisher();
        using var provider = CreateServiceProvider(publisher);
        var messageId = Guid.NewGuid();

        using (var seedScope = provider.CreateScope())
        {
            var dbContext = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Id = messageId,
                Type = typeof(CoursePublishedEvent).FullName!,
                Payload = $$"""{"courseId":"{{Guid.NewGuid()}}","occurredOnUTC":"{{DateTime.UtcNow:O}}"}""",
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

        publisher.PublishedMessages.Should().ContainSingle();
        publisher.PublishedMessages[0].MessageId.Should().Be(messageId);
        publisher.PublishedMessages[0].RoutingKey.Should().Be(OutboxIntegrationEventMapper.CoursePublishedRoutingKey);

        using (var assertScope = provider.CreateScope())
        {
            var dbContext = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var message = dbContext.OutboxMessages.Single();
            message.ProcessedOnUtc.Should().NotBeNull();
            message.Error.Should().BeNull();
        }
    }

    [Fact]
    public async Task DispatchPendingAsync_ShouldRecordRetryAndKeepMessageUnprocessedWhenPublishFails()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var provider = CreateServiceProvider(
            new FakeIntegrationEventPublisher(new InvalidOperationException("RabbitMQ publish failed.")));

        using (var seedScope = provider.CreateScope())
        {
            var dbContext = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Type = typeof(CoursePublishedEvent).FullName!,
                Payload = $$"""{"courseId":"{{Guid.NewGuid()}}","occurredOnUTC":"{{DateTime.UtcNow:O}}"}""",
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
            message.ProcessedOnUtc.Should().BeNull();
            message.RetryCount.Should().Be(1);
            message.Error.Should().Contain("RabbitMQ publish failed.");
        }
    }

    [Fact]
    public async Task DispatchPendingAsync_ShouldSkipUnsupportedMessageWithoutPublishing()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var publisher = new FakeIntegrationEventPublisher();
        using var provider = CreateServiceProvider(publisher);

        using (var seedScope = provider.CreateScope())
        {
            var dbContext = seedScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            dbContext.OutboxMessages.Add(new OutboxMessage
            {
                Type = "ELearning.Domain.Entities.CourseAggregate.Events.CourseCreatedEvent",
                Payload = $$"""{"courseId":"{{Guid.NewGuid()}}","occurredOnUTC":"{{DateTime.UtcNow:O}}"}""",
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

        publisher.PublishedMessages.Should().BeEmpty();

        using (var assertScope = provider.CreateScope())
        {
            var dbContext = assertScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var message = dbContext.OutboxMessages.Single();
            message.ProcessedOnUtc.Should().NotBeNull();
            message.Error.Should().BeNull();
        }
    }

    private static ServiceProvider CreateServiceProvider(FakeIntegrationEventPublisher publisher)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared",
                ["JwtSettings:Issuer"] = "integration-tests",
                ["JwtSettings:Audience"] = "integration-tests",
                ["JwtSettings:Secret"] = "integration-tests-secret-key-with-32chars",
                ["JwtSettings:ExpiryInDays"] = "7",
                ["RabbitMq:Enabled"] = "false",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.RemoveAll<IIntegrationEventPublisher>();
        services.AddSingleton<IIntegrationEventPublisher>(publisher);

        return services.BuildServiceProvider();
    }

    private sealed class FakeIntegrationEventPublisher(Exception? exception = null) : IIntegrationEventPublisher
    {
        public List<IntegrationEventPublishMessage> PublishedMessages { get; } = [];

        public Task PublishAsync(IntegrationEventPublishMessage message, CancellationToken cancellationToken)
        {
            if (exception is not null)
            {
                throw exception;
            }

            this.PublishedMessages.Add(message);
            return Task.CompletedTask;
        }
    }
}
