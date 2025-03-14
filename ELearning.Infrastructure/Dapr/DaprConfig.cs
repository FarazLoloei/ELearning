using ELearning.Application.Courses.Abstractions.ReadModels;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Instructors.Abstractions.ReadModels;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Submissions.Abstractions.ReadModels;
using ELearning.Infrastructure.ReadModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearning.Infrastructure.DaprServices;

/// <summary>
/// Configuration class for registering Dapr-related services
/// </summary>
public static class DaprConfig
{
    /// <summary>
    /// Adds Dapr client and related services to the service collection
    /// </summary>
    public static IServiceCollection AddDaprServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add required NuGet packages:
        // - Dapr.Client
        // - Dapr.AspNetCore

        // Register the DaprClient
        services.AddDaprClient(builder =>
        {
            // Optional: Configure retry policy
            builder.UseHttpEndpoint($"http://localhost:{configuration["Dapr:HttpPort"] ?? "3500"}");
            builder.UseGrpcEndpoint($"http://localhost:{configuration["Dapr:GrpcPort"] ?? "50001"}");
        });

        // Register custom Dapr service implementations for read operations
        services.AddSingleton<ICourseReadService, CourseReadService>();
        services.AddSingleton<IEnrollmentReadService, EnrollmentReadService>();
        services.AddSingleton<IStudentReadService, StudentReadService>();
        services.AddSingleton<IInstructorReadService, InstructorReadService>();
        services.AddSingleton<ISubmissionReadService, SubmissionReadService>();

        return services;
    }
}