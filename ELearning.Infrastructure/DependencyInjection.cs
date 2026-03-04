using ELearning.Application.Common.Interfaces;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Services;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Repositories;
using ELearning.Infrastructure.Outbox;
using ELearning.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearning.Infrastructure;

public static class DependencyInjection
{
    private const string SqliteInMemoryProvider = "SqliteInMemory";
    private const string SqlServerProvider = "SqlServer";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var databaseProvider = configuration["Database:Provider"] ?? SqliteInMemoryProvider;

        if (databaseProvider.Equals(SqliteInMemoryProvider, StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration["Database:SqliteInMemoryConnection"] ?? "Data Source=:memory:;Cache=Shared";

            services.AddSingleton(_ =>
            {
                var connection = new SqliteConnection(connectionString);
                connection.Open();
                return connection;
            });

            services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
            {
                var connection = serviceProvider.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });
        }
        else if (databaseProvider.Equals(SqlServerProvider, StringComparison.OrdinalIgnoreCase))
        {
            var defaultConnection = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required when using SqlServer provider.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    defaultConnection,
                    sqlServerOptions => sqlServerOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }
        else
        {
            throw new InvalidOperationException($"Unsupported Database:Provider '{databaseProvider}'. Supported values: {SqliteInMemoryProvider}, {SqlServerProvider}.");
        }

        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IEnrollmentReadRepository, EnrollmentReadRepository>();
        services.AddScoped<IModuleRepository, ModuleRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IProgressReadRepository, ProgressRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAssignmentService, AssignmentService>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IOutboxDispatcher, OutboxDispatcher>();
        services.AddHostedService<OutboxDispatcherHostedService>();

        return services;
    }
}
