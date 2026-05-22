// <copyright file="RabbitMqOptionsValidator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Options;

using Microsoft.Extensions.Options;

public sealed class RabbitMqOptionsValidator : IValidateOptions<RabbitMqOptions>
{
    public ValidateOptionsResult Validate(string? name, RabbitMqOptions options)
    {
        if (!options.Enabled)
        {
            return ValidateOptionsResult.Success;
        }

        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.HostName))
        {
            failures.Add("RabbitMq:HostName is required when RabbitMQ is enabled.");
        }

        if (options.Port is < 1 or > 65535)
        {
            failures.Add("RabbitMq:Port must be between 1 and 65535 when RabbitMQ is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.UserName))
        {
            failures.Add("RabbitMq:UserName is required when RabbitMQ is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.Password))
        {
            failures.Add("RabbitMq:Password is required when RabbitMQ is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.VirtualHost))
        {
            failures.Add("RabbitMq:VirtualHost is required when RabbitMQ is enabled.");
        }

        if (string.IsNullOrWhiteSpace(options.ExchangeName))
        {
            failures.Add("RabbitMq:ExchangeName is required when RabbitMQ is enabled.");
        }

        if (options.PublisherConfirmTimeoutSeconds <= 0)
        {
            failures.Add("RabbitMq:PublisherConfirmTimeoutSeconds must be greater than zero when RabbitMQ is enabled.");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
