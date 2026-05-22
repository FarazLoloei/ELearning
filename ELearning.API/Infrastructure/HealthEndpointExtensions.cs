// <copyright file="HealthEndpointExtensions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;

public static class HealthEndpointExtensions
{
    public static IEndpointRouteBuilder MapApiHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHealthChecks(
                "/health/live",
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("live", StringComparer.OrdinalIgnoreCase),
                    ResponseWriter = HealthCheckResponseWriter.WriteAsync,
                })
            .AllowAnonymous();

        endpoints.MapHealthChecks(
                "/health/ready",
                new HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready", StringComparer.OrdinalIgnoreCase),
                    ResponseWriter = HealthCheckResponseWriter.WriteAsync,
                })
            .AllowAnonymous();

        return endpoints;
    }
}
