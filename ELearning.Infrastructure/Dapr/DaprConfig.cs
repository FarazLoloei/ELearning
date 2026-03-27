// <copyright file="DaprConfig.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.DaprServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Configuration class for registering Dapr-related services.
/// </summary>
public static class DaprConfig
{
    internal const string SidecarHttpClientName = "DaprSidecar";

    /// <summary>
    /// Adds Dapr client to the service collection.
    /// </summary>
    /// <returns></returns>
    public static IServiceCollection AddDaprServices(this IServiceCollection services, IConfiguration configuration)
    {
        var daprHttpEndpoint = $"http://localhost:{configuration["Dapr:HttpPort"] ?? "3500"}";

        services.AddDaprClient(builder =>
        {
            builder.UseHttpEndpoint(daprHttpEndpoint);
            builder.UseGrpcEndpoint($"http://localhost:{configuration["Dapr:GrpcPort"] ?? "50001"}");
        });

        services.AddHttpClient(SidecarHttpClientName, client =>
        {
            client.BaseAddress = new Uri($"{daprHttpEndpoint}/v1.0/invoke/");
        });

        return services;
    }
}
