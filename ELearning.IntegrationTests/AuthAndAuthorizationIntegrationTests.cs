// <copyright file="AuthAndAuthorizationIntegrationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using ELearning.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public sealed class AuthAndAuthorizationIntegrationTests : IClassFixture<RealAuthWebApplicationFactory>
{
    private const string JwtIssuer = "integration-tests";
    private const string JwtAudience = "integration-tests";
    private const string JwtSecret = "integration-tests-secret-key-with-32chars";

    private readonly RealAuthWebApplicationFactory factory;
    private readonly HttpClient client;

    public AuthAndAuthorizationIntegrationTests(RealAuthWebApplicationFactory factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_WithRealAuthService_ReturnsJwtToken()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var email = $"auth.login.{Guid.NewGuid():N}@tests.io";
        const string password = "P@ssword123!";

        await this.SeedStudentAsync(email, password, cancellationToken);

        var response = await this.client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password,
        }, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var content = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        content.RootElement.GetProperty("succeeded").GetBoolean().Should().BeTrue();

        var authResult = content.RootElement.GetProperty("data");
        authResult.GetProperty("success").GetBoolean().Should().BeTrue();
        var token = authResult.GetProperty("token").GetString();
        var refreshToken = authResult.GetProperty("refreshToken").GetString();
        token.Should().NotBeNullOrWhiteSpace();
        token.Should().NotBe("stub-token");
        refreshToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task Refresh_And_RevokeTokenFlow_ShouldRotateAndInvalidateRefreshToken()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var email = $"refresh.flow.{Guid.NewGuid():N}@tests.io";
        const string password = "P@ssword123!";

        await this.SeedStudentAsync(email, password, cancellationToken);

        var loginResponse = await this.client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password,
        }, cancellationToken);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var loginStream = await loginResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var loginJson = await JsonDocument.ParseAsync(loginStream, cancellationToken: cancellationToken);
        var firstRefreshToken = loginJson.RootElement.GetProperty("data").GetProperty("refreshToken").GetString();
        firstRefreshToken.Should().NotBeNullOrWhiteSpace();

        var refreshResponse = await this.client.PostAsJsonAsync("/api/auth/refresh", new
        {
            RefreshToken = firstRefreshToken,
        }, cancellationToken);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var refreshStream = await refreshResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var refreshJson = await JsonDocument.ParseAsync(refreshStream, cancellationToken: cancellationToken);
        var accessToken = refreshJson.RootElement.GetProperty("data").GetProperty("token").GetString();
        var rotatedRefreshToken = refreshJson.RootElement.GetProperty("data").GetProperty("refreshToken").GetString();
        accessToken.Should().NotBeNullOrWhiteSpace();
        rotatedRefreshToken.Should().NotBeNullOrWhiteSpace();
        rotatedRefreshToken.Should().NotBe(firstRefreshToken);

        using var revokeRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/revoke")
        {
            Content = JsonContent.Create(new
            {
                RefreshToken = rotatedRefreshToken
            }),
        };
        revokeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var revokeResponse = await this.client.SendAsync(revokeRequest, cancellationToken);
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var revokedRefreshResponse = await this.client.PostAsJsonAsync("/api/auth/refresh", new
        {
            RefreshToken = rotatedRefreshToken,
        }, cancellationToken);
        revokedRefreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var auditTypes = dbContext.SecurityAuditEvents
            .Select(x => x.EventType)
            .ToList();
        auditTypes.Should().Contain("auth.login");
        auditTypes.Should().Contain("auth.refresh");
        auditTypes.Should().Contain("auth.revoke");
    }

    [Fact]
    public async Task Revoke_WithDifferentAuthenticatedUser_ShouldReturnForbidden()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var ownerEmail = $"revoke.owner.{Guid.NewGuid():N}@tests.io";
        const string ownerPassword = "P@ssword123!";
        await this.SeedStudentAsync(ownerEmail, ownerPassword, cancellationToken);

        var loginResponse = await this.client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = ownerEmail,
            Password = ownerPassword,
        }, cancellationToken);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using var loginStream = await loginResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var loginJson = await JsonDocument.ParseAsync(loginStream, cancellationToken: cancellationToken);
        var ownerRefreshToken = loginJson.RootElement.GetProperty("data").GetProperty("refreshToken").GetString();
        ownerRefreshToken.Should().NotBeNullOrWhiteSpace();

        using var revokeRequest = new HttpRequestMessage(HttpMethod.Post, "/api/auth/revoke")
        {
            Content = JsonContent.Create(new
            {
                RefreshToken = ownerRefreshToken
            }),
        };
        revokeRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(Guid.NewGuid(), "Student"));

        var revokeResponse = await this.client.SendAsync(revokeRequest, cancellationToken);
        revokeResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var response = await this.client.GetAsync($"/api/students/{Guid.NewGuid()}/progress", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InstructorEndpoint_WithStudentToken_ReturnsForbidden()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var studentId = Guid.NewGuid();
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/instructors/{Guid.NewGuid()}/pending-submissions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(studentId, "Student"));

        var response = await this.client.SendAsync(request, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task StudentProgress_WithAnotherStudentToken_ReturnsForbidden()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var tokenOwnerId = Guid.NewGuid();
        var requestedStudentId = Guid.NewGuid();

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/students/{requestedStudentId}/progress");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(tokenOwnerId, "Student"));

        var response = await this.client.SendAsync(request, cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> SeedStudentAsync(string email, string password, CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userService = scope.ServiceProvider.GetRequiredService<ELearning.Domain.Entities.UserAggregate.Abstractions.Services.IUserService>();

        var student = new Student(
            firstName: "Integration",
            lastName: "Student",
            email: Email.Create(email),
            passwordHash: userService.HashPassword(password));

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync(cancellationToken);
        return student.Id;
    }

    private static string CreateJwt(Guid userId, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
