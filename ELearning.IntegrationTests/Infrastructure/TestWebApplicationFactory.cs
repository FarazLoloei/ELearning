using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.UserAggregate;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ELearning.IntegrationTests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var testSettings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=(localdb)\\mssqllocaldb;Database=ELearning_IntegrationTests;Trusted_Connection=True;",
                ["JwtSettings:Issuer"] = "integration-tests",
                ["JwtSettings:Audience"] = "integration-tests",
                ["JwtSettings:Secret"] = "integration-tests-secret-key-with-32chars",
                ["JwtSettings:ExpiryInDays"] = "7"
            };

            configBuilder.AddInMemoryCollection(testSettings);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAuthService>();
            services.AddScoped<IAuthService, StubAuthService>();
        });
    }

    private sealed class StubAuthService : IAuthService
    {
        public Task<AuthResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var payload = new AuthPayload(
                "stub-token",
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                email,
                "Integration Test User",
                "Student");

            return Task.FromResult(AuthResult.Succeeded(payload));
        }

        public Task<AuthResult> RegisterStudentAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken)
            => Task.FromResult(AuthResult.Failed("Not used in this integration test."));

        public Task<AuthResult> RegisterInstructorAsync(string firstName, string lastName, string email, string password, string bio, string expertise, CancellationToken cancellationToken)
            => Task.FromResult(AuthResult.Failed("Not used in this integration test."));

        public Task<string> GenerateJwtToken(User user)
            => Task.FromResult("stub-token");
    }
}