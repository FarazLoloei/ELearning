// <copyright file="RabbitMqIntegrationEventPublisher.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using System.Text;
using ELearning.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

public sealed class RabbitMqIntegrationEventPublisher(
    IOptions<RabbitMqOptions> rabbitMqOptions,
    ILogger<RabbitMqIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    private const string ContentType = "application/json";

    private readonly RabbitMqOptions options = rabbitMqOptions.Value;

    public async Task PublishAsync(IntegrationEventPublishMessage message, CancellationToken cancellationToken)
    {
        using var timeoutTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutTokenSource.CancelAfter(TimeSpan.FromSeconds(this.options.PublisherConfirmTimeoutSeconds));

        var factory = new ConnectionFactory
        {
            HostName = this.options.HostName,
            Port = this.options.Port,
            UserName = this.options.UserName,
            Password = this.options.Password,
            VirtualHost = this.options.VirtualHost,
            ClientProvidedName = "elearning-api-outbox-publisher",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
        };

        await using var connection = await factory.CreateConnectionAsync(timeoutTokenSource.Token);
        await using var channel = await connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true),
            timeoutTokenSource.Token);

        await channel.ExchangeDeclareAsync(
            exchange: this.options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: timeoutTokenSource.Token);

        var properties = new BasicProperties
        {
            ContentType = ContentType,
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = message.MessageId.ToString("D"),
            Type = message.EventType,
        };

        var body = Encoding.UTF8.GetBytes(message.Payload);

        await channel.BasicPublishAsync(
            exchange: this.options.ExchangeName,
            routingKey: message.RoutingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: timeoutTokenSource.Token);

        logger.LogInformation(
            "Published integration event {EventType} with message id {MessageId} to RabbitMQ routing key {RoutingKey}.",
            message.EventType,
            message.MessageId,
            message.RoutingKey);
    }
}
