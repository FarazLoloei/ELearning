// <copyright file="TestWebApplicationFactory.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests.Infrastructure;

using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Model;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared",
                ["JwtSettings:Issuer"] = "integration-tests",
                ["JwtSettings:Audience"] = "integration-tests",
                ["JwtSettings:Secret"] = "integration-tests-secret-key-with-32chars",
                ["JwtSettings:ExpiryInDays"] = "7",
            };

            configBuilder.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IRequestHandler<AuthenticateUserCommand, AuthResult>>();
            services.AddScoped<IRequestHandler<AuthenticateUserCommand, AuthResult>, StubAuthenticateUserCommandHandler>();
        });
    }

    private sealed class StubAuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, AuthResult>
    {
        public Task<AuthResult> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
        {
            var payload = new AuthPayload(
                "stub-token",
                "stub-refresh-token",
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                request.Email,
                "Integration Test User",
                "Student");

            return Task.FromResult(AuthResult.Succeeded(payload));
        }
    }
}
