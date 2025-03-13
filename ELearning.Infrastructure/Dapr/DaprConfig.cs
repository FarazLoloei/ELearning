using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearning.Infrastructure.DaprServices;

public static class DaprConfig
{
    public static IServiceCollection AddDaprServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Dapr client
        services.AddDaprClient();

        // Register Dapr service implementations
        services.AddSingleton<ICourseReadService, CourseReadService>();
        services.AddSingleton<IEnrollmentReadService, EnrollmentReadService>();
        services.AddSingleton<IUserReadService, UserReadService>();
        services.AddSingleton<ISubmissionReadService, SubmissionReadService>();

        return services;
    }
}