// <copyright file="RealAuthWebApplicationFactory.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public sealed class RealAuthWebApplicationFactory : WebApplicationFactory<Program>
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
                ["Ocelot:Enabled"] = "false",
            };

            configBuilder.AddInMemoryCollection(testSettings);
        });
    }
}
