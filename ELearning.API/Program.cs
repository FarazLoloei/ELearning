// <copyright file="Program.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

using ELearning.API.GraphQL;
using ELearning.API.Infrastructure;
using ELearning.API.Middleware;
using ELearning.Application;
using ELearning.Infrastructure;
using ELearning.Infrastructure.DaprServices;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddApiConfigurationSources(builder.Environment);

var ocelotGatewayEnabled = OcelotGatewayMode.IsEnabled(builder.Configuration);

builder.Services.AddApiConfigurationValidation(builder.Configuration);
builder.AddApiObservability();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddDaprServices(builder.Configuration);
builder.Services.AddGraphQLServices();

builder.Services.AddApiPresentation();
builder.Services.AddApiCors(builder.Configuration, builder.Environment);
builder.Services.AddApiGateway(builder.Configuration, ocelotGatewayEnabled);
builder.Services.AddApiAuthentication();
builder.Services.AddApiRateLimiting();
builder.Services.AddApiHealthChecks();

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Configuration);

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

app.MapApiHealthChecks();

if (!ocelotGatewayEnabled)
{
    app.MapControllers();
    app.MapGraphQL();
}
else
{
    await app.UseOcelot();
}

app.Run();
