// <copyright file="ApiServiceCollectionExtensions.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.Infrastructure;

using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ELearning.API.Facades;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;

public static class ApiServiceCollectionExtensions
{
    public static IServiceCollection AddApiPresentation(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
                new Asp.Versioning.UrlSegmentApiVersionReader(),
                new Asp.Versioning.HeaderApiVersionReader("api-version"));
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "E-Learning Platform API",
                Version = "v1",
                Description = "REST-first API for the E-Learning platform. GraphQL is available as a secondary interface at /graphql.",
                Contact = new OpenApiContact
                {
                    Name = "E-Learning API",
                },
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document, null!),
                    new List<string>()
                },
            });
        });

        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(entry => entry.Value?.Errors.Count > 0)
                    .ToDictionary(
                        entry => entry.Key,
                        entry => entry.Value!.Errors
                            .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? "The input was not valid."
                                : error.ErrorMessage)
                            .ToArray());

                var problemDetails = ApiProblemDetailsFactory.CreateValidation(context.HttpContext, errors);
                return ApiProblemDetailsFactory.ToObjectResult(problemDetails);
            };
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IApiFacade, ApiFacade>();

        return services;
    }

    public static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var allowedCorsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy(
                "CorsPolicy",
                policyBuilder =>
                {
                    if (allowedCorsOrigins is { Length: > 0 })
                    {
                        policyBuilder
                            .WithOrigins(allowedCorsOrigins)
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                    else if (environment.IsDevelopment())
                    {
                        policyBuilder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    }
                });
        });

        return services;
    }

    public static IServiceCollection AddApiGateway(
        this IServiceCollection services,
        IConfiguration configuration,
        bool ocelotGatewayEnabled)
    {
        if (!ocelotGatewayEnabled)
        {
            return services;
        }

        services.AddOcelot(configuration)
            .AddCacheManager(settings => settings.WithDictionaryHandle());

        return services;
    }

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, JwtBearerOptionsSetup>();

        return services;
    }

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("AuthEndpoints", limiterOptions =>
            {
                limiterOptions.PermitLimit = 10;
                limiterOptions.Window = TimeSpan.FromMinutes(1);
                limiterOptions.QueueLimit = 0;
                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });

        return services;
    }

    public static IServiceCollection AddApiHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddDbContextCheck<ApplicationDbContext>(
                "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: ["ready"],
                customTestQuery: (dbContext, cancellationToken) => dbContext.Database.CanConnectAsync(cancellationToken));

        return services;
    }
}
