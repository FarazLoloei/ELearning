using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ELearning.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace ELearning.IntegrationTests;

public sealed class AuthControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WhenCredentialsProvided_ReturnsOkWithAuthPayload()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var request = new
        {
            Email = "sample.user@elearning.test",
            Password = "P@ssword123!"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var content = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        content.RootElement.GetProperty("succeeded").GetBoolean().Should().BeTrue();

        var authResult = content.RootElement.GetProperty("data");
        authResult.GetProperty("success").GetBoolean().Should().BeTrue();
        authResult.GetProperty("token").GetString().Should().Be("stub-token");
        authResult.GetProperty("email").GetString().Should().Be("sample.user@elearning.test");
    }
}
