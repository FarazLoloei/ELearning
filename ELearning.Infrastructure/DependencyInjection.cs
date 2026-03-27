// <copyright file="DependencyInjection.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure;

using ELearning.Application.Auth.Abstractions;
using ELearning.Application.Certificates.Abstractions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Students.Abstractions;
using ELearning.Application.Submissions.Abstractions;
using ELearning.Domain.Entities.CertificateAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Repositories;
using ELearning.Infrastructure.Outbox;
using ELearning.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddScoped<ICertificateRepository, CertificateRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserReadRepository, UserReadRepository>();
        services.AddScoped<IStudentReadRepository, StudentReadRepository>();
        services.AddScoped<IInstructorReadRepository, InstructorReadRepository>();
        services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        services.AddScoped<IEnrollmentReadRepository, EnrollmentReadRepository>();
        services.AddScoped<ICertificateReadRepository, CertificateReadRepository>();
        services.AddScoped<ICourseReadRepository, CourseReadRepository>();
        services.AddScoped<IAssignmentReadRepository, AssignmentReadRepository>();
        services.AddScoped<ISubmissionReadRepository, SubmissionReadRepository>();
        services.AddScoped<IProgressReadRepository, ProgressReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileStorageService, FileStorageService>();
        services.AddScoped<IAccessTokenIssuer, AccessTokenIssuer>();
        services.AddScoped<IRefreshTokenStore, RefreshTokenStore>();
        services.AddScoped<ISecurityAuditWriter, SecurityAuditWriter>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IOutboxDispatcher, OutboxDispatcher>();
        services.AddHostedService<OutboxDispatcherHostedService>();

        return services;
    }
}
