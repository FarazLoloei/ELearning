// <copyright file="ApiConfigurationExtensions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using ELearning.Infrastructure.Options;
using Microsoft.Extensions.Options;

public static class ApiConfigurationExtensions
{
    public static IConfigurationBuilder AddApiConfigurationSources(
        this IConfigurationBuilder configuration,
        IWebHostEnvironment environment)
    {
        return configuration
            .SetBasePath(environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("ocelot.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"ocelot.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    }

    public static IServiceCollection AddApiConfigurationValidation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IValidateOptions<DatabaseOptions>, DatabaseOptionsValidator>();
        services.AddSingleton<IValidateOptions<JwtSettingsOptions>, JwtSettingsOptionsValidator>();

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<JwtSettingsOptions>()
            .Bind(configuration.GetSection(JwtSettingsOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<ObservabilityOptions>()
            .Bind(configuration.GetSection(ObservabilityOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ServiceName), "Observability:ServiceName is required.")
            .Validate(
                options => string.IsNullOrWhiteSpace(options.OtlpEndpoint) ||
                           Uri.TryCreate(options.OtlpEndpoint, UriKind.Absolute, out _),
                "Observability:OtlpEndpoint must be an absolute URI when configured.")
            .ValidateOnStart();

        return services;
    }
}
