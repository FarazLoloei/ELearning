// <copyright file="ObservabilityConfiguration.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using System.Reflection;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

public static class ObservabilityConfiguration
{
    public static WebApplicationBuilder AddApiObservability(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection(ObservabilityOptions.SectionName).Get<ObservabilityOptions>()
            ?? new ObservabilityOptions();

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => ConfigureResource(resource, options, builder.Environment))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation(instrumentation =>
                    {
                        instrumentation.Filter = context =>
                            !context.Request.Path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
                    })
                    .AddHttpClientInstrumentation();

                AddTraceExporters(tracing, options);
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation();

                AddMetricExporters(metrics, options);
            });

        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.SetResourceBuilder(CreateResourceBuilder(options, builder.Environment));
            AddLogExporters(logging, options);
        });

        return builder;
    }

    private static ResourceBuilder ConfigureResource(
        ResourceBuilder resourceBuilder,
        ObservabilityOptions options,
        IWebHostEnvironment environment)
    {
        return resourceBuilder
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString())
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = environment.EnvironmentName,
            });
    }

    private static ResourceBuilder CreateResourceBuilder(
        ObservabilityOptions options,
        IWebHostEnvironment environment) =>
        ConfigureResource(ResourceBuilder.CreateDefault(), options, environment);

    private static void AddTraceExporters(TracerProviderBuilder tracing, ObservabilityOptions options)
    {
        if (options.ConsoleExporterEnabled)
        {
            tracing.AddConsoleExporter();
        }

        if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
        {
            tracing.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
        }
    }

    private static void AddMetricExporters(MeterProviderBuilder metrics, ObservabilityOptions options)
    {
        if (options.ConsoleExporterEnabled)
        {
            metrics.AddConsoleExporter();
        }

        if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
        {
            metrics.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
        }
    }

    private static void AddLogExporters(OpenTelemetryLoggerOptions logging, ObservabilityOptions options)
    {
        if (options.ConsoleExporterEnabled)
        {
            logging.AddConsoleExporter();
        }

        if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint))
        {
            logging.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(options.OtlpEndpoint));
        }
    }
}
