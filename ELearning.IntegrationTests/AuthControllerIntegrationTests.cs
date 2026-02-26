using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ELearning.IntegrationTests.Infrastructure;

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

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var content = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        Assert.True(content.RootElement.GetProperty("succeeded").GetBoolean());

        var authResult = content.RootElement.GetProperty("data");
        Assert.True(authResult.GetProperty("success").GetBoolean());
        Assert.Equal("stub-token", authResult.GetProperty("token").GetString());
        Assert.Equal("sample.user@elearning.test", authResult.GetProperty("email").GetString());
    }
}
