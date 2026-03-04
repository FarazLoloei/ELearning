using ELearning.API.GraphQL;
using ELearning.API.Facades;
using ELearning.API.Infrastructure;
using ELearning.API.Middleware;
using ELearning.Application;
using ELearning.Infrastructure;
using ELearning.Infrastructure.DaprServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Reflection;
using System.Threading.RateLimiting;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add application layer
builder.Services.AddApplication();

// Add infrastructure layer (includes CurrentUserService)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Dapr services for read operations
builder.Services.AddDaprServices(builder.Configuration);

// Add GraphQL services
builder.Services.AddGraphQLServices();

// Configure configuration sources
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

var ocelotGatewayEnabled = OcelotGatewayMode.IsEnabled(builder.Configuration);

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = Asp.Versioning.ApiVersionReader.Combine(
        new Asp.Versioning.UrlSegmentApiVersionReader(),
        new Asp.Versioning.HeaderApiVersionReader("api-version"));
}).AddApiExplorer(options =>
{
    // Format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add CORS
var allowedCorsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        policyBuilder =>
        {
            if (allowedCorsOrigins is { Length: > 0 })
            {
                policyBuilder
                    .WithOrigins(allowedCorsOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }
            else if (builder.Environment.IsDevelopment())
            {
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }
        });
});

// Add Ocelot only when running in gateway mode
if (ocelotGatewayEnabled)
{
    builder.Services.AddOcelot(builder.Configuration)
        .AddCacheManager(settings => settings.WithDictionaryHandle());
}

// Add authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured")))
        };
    });

builder.Services.AddRateLimiter(options =>
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

// Add swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Learning Platform API",
        Version = "v1",
        Description = "REST-first API for the E-Learning platform. GraphQL is available as a secondary interface at /graphql.",
        Contact = new OpenApiContact
        {
            Name = "E-Learning API"
        }
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
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Add HttpContextAccessor for CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IApiFacade, ApiFacade>();

// Build the app
var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.RoutePrefix = string.Empty;
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Learning Platform REST API v1");
    c.DocumentTitle = "E-Learning REST API Docs";
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseRateLimiter();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints for monolith mode; gateway mode delegates to Ocelot pipeline.
if (!ocelotGatewayEnabled)
{
    app.MapControllers(); // REST API endpoints
    app.MapGraphQL();     // GraphQL endpoint at /graphql
}
else
{
    await app.UseOcelot();
}

// Run the app
app.Run();

public partial class Program;
