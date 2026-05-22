// <copyright file="RabbitMqOptions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public bool Enabled { get; init; }

    public string HostName { get; init; } = "localhost";

    public int Port { get; init; } = 5672;

    public string UserName { get; init; } = "guest";

    public string Password { get; init; } = "guest";

    public string VirtualHost { get; init; } = "/";

    public string ExchangeName { get; init; } = "elearning.integration.events";

    public string NotificationQueueName { get; init; } = "elearning.notifications.email";

    public string DeadLetterExchangeName { get; init; } = "elearning.integration.dead-letter";

    public string DeadLetterQueueName { get; init; } = "elearning.notifications.email.dead-letter";

    public int PublisherConfirmTimeoutSeconds { get; init; } = 5;
}
