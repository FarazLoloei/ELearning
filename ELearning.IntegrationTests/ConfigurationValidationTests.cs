// <copyright file="ConfigurationValidationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Infrastructure;
using ELearning.Infrastructure.Notifications;
using ELearning.Infrastructure.Options;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public sealed class ConfigurationValidationTests
{
    [Fact]
    public void DatabaseOptionsValidator_ShouldRejectUnsupportedProvider()
    {
        var configuration = new ConfigurationBuilder().Build();
        var validator = new DatabaseOptionsValidator(configuration);

        var result = validator.Validate(null, new DatabaseOptions { Provider = "Postgres" });

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(failure => failure.Contains("Database:Provider", StringComparison.Ordinal));
    }

    [Fact]
    public void DatabaseOptionsValidator_ShouldRequireSqlServerConnectionString()
    {
        var configuration = new ConfigurationBuilder().Build();
        var validator = new DatabaseOptionsValidator(configuration);

        var result = validator.Validate(null, new DatabaseOptions { Provider = DatabaseProviderNames.SqlServer });

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(failure => failure.Contains("ConnectionStrings:DefaultConnection", StringComparison.Ordinal));
    }

    [Fact]
    public void JwtSettingsOptionsValidator_ShouldRequireStrongStartupConfiguration()
    {
        var validator = new JwtSettingsOptionsValidator();

        var result = validator.Validate(
            null,
            new JwtSettingsOptions
            {
                Issuer = "integration-tests",
                Audience = "integration-tests",
                Secret = "short",
                ExpiryInDays = 7,
            });

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(failure => failure.Contains("JwtSettings:Secret", StringComparison.Ordinal));
    }

    [Fact]
    public void RabbitMqOptionsValidator_ShouldAllowDisabledOptionsWithoutBrokerSettings()
    {
        var validator = new RabbitMqOptionsValidator();

        var result = validator.Validate(
            null,
            new RabbitMqOptions
            {
                Enabled = false,
                HostName = string.Empty,
                UserName = string.Empty,
                Password = string.Empty,
                VirtualHost = string.Empty,
                ExchangeName = string.Empty,
                PublisherConfirmTimeoutSeconds = 0,
            });

        result.Failed.Should().BeFalse();
    }

    [Fact]
    public void RabbitMqOptionsValidator_ShouldRequireBrokerSettingsWhenEnabled()
    {
        var validator = new RabbitMqOptionsValidator();

        var result = validator.Validate(
            null,
            new RabbitMqOptions
            {
                Enabled = true,
                HostName = string.Empty,
                Port = 0,
                UserName = string.Empty,
                Password = string.Empty,
                VirtualHost = string.Empty,
                ExchangeName = string.Empty,
                NotificationQueueName = string.Empty,
                DeadLetterExchangeName = string.Empty,
                DeadLetterQueueName = string.Empty,
                PublisherConfirmTimeoutSeconds = 0,
            });

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:HostName", StringComparison.Ordinal));
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:ExchangeName", StringComparison.Ordinal));
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:NotificationQueueName", StringComparison.Ordinal));
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:DeadLetterExchangeName", StringComparison.Ordinal));
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:DeadLetterQueueName", StringComparison.Ordinal));
        result.Failures.Should().Contain(failure => failure.Contains("RabbitMq:PublisherConfirmTimeoutSeconds", StringComparison.Ordinal));
    }

    [Fact]
    public void AddInfrastructure_WhenRabbitMqIsDisabled_ShouldNotRegisterNotificationConsumerHostedService()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared",
                ["RabbitMq:Enabled"] = "false",
            })
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        services.Should().NotContain(
            descriptor => descriptor.ServiceType == typeof(IHostedService) &&
                          descriptor.ImplementationType == typeof(RabbitMqNotificationConsumerHostedService));
    }
}
