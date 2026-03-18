using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearning.Infrastructure.DaprServices;

/// <summary>
/// Configuration class for registering Dapr-related services.
/// </summary>
public static class DaprConfig
{
    /// <summary>
    /// Adds Dapr client to the service collection.
    /// </summary>
    public static IServiceCollection AddDaprServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDaprClient(builder =>
        {
            builder.UseHttpEndpoint($"http://localhost:{configuration["Dapr:HttpPort"] ?? "3500"}");
            builder.UseGrpcEndpoint($"http://localhost:{configuration["Dapr:GrpcPort"] ?? "50001"}");
        });

        return services;
    }
}
