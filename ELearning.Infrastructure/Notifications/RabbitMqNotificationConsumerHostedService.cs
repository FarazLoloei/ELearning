// <copyright file="RabbitMqNotificationConsumerHostedService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Notifications;

using System.Text;
using ELearning.Infrastructure.Options;
using ELearning.Infrastructure.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class RabbitMqNotificationConsumerHostedService(
    IServiceScopeFactory scopeFactory,
    IOptions<RabbitMqOptions> rabbitMqOptions,
    ILogger<RabbitMqNotificationConsumerHostedService> logger) : BackgroundService
{
    private readonly RabbitMqOptions options = rabbitMqOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = this.options.HostName,
            Port = this.options.Port,
            UserName = this.options.UserName,
            Password = this.options.Password,
            VirtualHost = this.options.VirtualHost,
            ClientProvidedName = "elearning-api-notification-consumer",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5),
        };

        await using var connection = await factory.CreateConnectionAsync(stoppingToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await this.DeclareTopologyAsync(channel, stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, args) =>
        {
            await this.HandleReceivedAsync(channel, args, stoppingToken);
        };

        await channel.BasicConsumeAsync(
            queue: this.options.NotificationQueueName,
            autoAck: false,
            consumer: consumer,
            cancellationToken: stoppingToken);

        logger.LogInformation(
            "RabbitMQ notification consumer started for queue {QueueName}.",
            this.options.NotificationQueueName);

        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }

    private async Task HandleReceivedAsync(
        IChannel channel,
        BasicDeliverEventArgs args,
        CancellationToken cancellationToken)
    {
        try
        {
            var payload = Encoding.UTF8.GetString(args.Body.Span);
            var eventType = args.BasicProperties.Type ?? nameof(NotificationRequestedIntegrationEvent);

            if (!Guid.TryParse(args.BasicProperties.MessageId, out var messageId))
            {
                logger.LogWarning(
                    "Rejecting notification message without a valid message id. Delivery tag: {DeliveryTag}",
                    args.DeliveryTag);
                await channel.BasicRejectAsync(args.DeliveryTag, requeue: false, cancellationToken);
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<INotificationIntegrationEventHandler>();
            await handler.HandleAsync(messageId, eventType, payload, cancellationToken);

            await channel.BasicAckAsync(args.DeliveryTag, multiple: false, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process RabbitMQ notification message with delivery tag {DeliveryTag}.",
                args.DeliveryTag);

            await channel.BasicRejectAsync(args.DeliveryTag, requeue: false, cancellationToken);
        }
    }

    private async Task DeclareTopologyAsync(IChannel channel, CancellationToken cancellationToken)
    {
        await channel.ExchangeDeclareAsync(
            exchange: this.options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: this.options.DeadLetterExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: this.options.DeadLetterQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: this.options.DeadLetterQueueName,
            exchange: this.options.DeadLetterExchangeName,
            routingKey: this.options.NotificationQueueName,
            cancellationToken: cancellationToken);

        var queueArguments = new Dictionary<string, object?>
        {
            ["x-dead-letter-exchange"] = this.options.DeadLetterExchangeName,
            ["x-dead-letter-routing-key"] = this.options.NotificationQueueName,
        };

        await channel.QueueDeclareAsync(
            queue: this.options.NotificationQueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: this.options.NotificationQueueName,
            exchange: this.options.ExchangeName,
            routingKey: OutboxIntegrationEventMapper.NotificationRequestedRoutingKey,
            cancellationToken: cancellationToken);
    }
}
