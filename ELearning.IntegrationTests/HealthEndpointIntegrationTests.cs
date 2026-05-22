// <copyright file="HealthEndpointIntegrationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.Net;
using System.Text.Json;
using ELearning.IntegrationTests.Infrastructure;
using FluentAssertions;

public sealed class HealthEndpointIntegrationTests : IClassFixture<RealAuthWebApplicationFactory>
{
    private readonly HttpClient client;

    public HealthEndpointIntegrationTests(RealAuthWebApplicationFactory factory)
    {
        this.client = factory.CreateClient();
    }

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task HealthEndpoint_ShouldReturnHealthyPayloadWithoutAuthentication(string endpoint)
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var response = await this.client.GetAsync(endpoint, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        var root = document.RootElement;

        root.GetProperty("status").GetString().Should().Be("Healthy");
        root.GetProperty("traceId").GetString().Should().NotBeNullOrWhiteSpace();
        root.GetProperty("entries").ValueKind.Should().Be(JsonValueKind.Object);
    }
}
