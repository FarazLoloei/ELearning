// <copyright file="ApiErrorContractIntegrationTests.cs" company="FarazLoloei">
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
using ELearning.Application.Auth.Abstractions;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using ELearning.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public sealed class ApiErrorContractIntegrationTests : IClassFixture<RealAuthWebApplicationFactory>
{
    private const string JwtIssuer = "integration-tests";
    private const string JwtAudience = "integration-tests";
    private const string JwtSecret = "integration-tests-secret-key-with-32chars";

    private readonly RealAuthWebApplicationFactory factory;
    private readonly HttpClient client;

    public ApiErrorContractIntegrationTests(RealAuthWebApplicationFactory factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task ValidationFailure_ReturnsValidationProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);

        var response = await this.PostAsRoleAsync(
            "/api/courses",
            new
            {
                Title = string.Empty,
                Description = string.Empty,
                CategoryId = 0,
                LevelId = 0,
                Price = -1m,
                DurationHours = -1,
                DurationMinutes = 60,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.BadRequest,
            "validation.failed",
            "Validation failed",
            cancellationToken);

        problem.RootElement.GetProperty("errors").TryGetProperty("Title", out _).Should().BeTrue();
    }

    [Fact]
    public async Task MissingResource_ReturnsNotFoundProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await this.client.GetAsync($"/api/courses/{Guid.NewGuid()}", cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.NotFound,
            "resource.not_found",
            "Resource not found",
            cancellationToken);
    }

    [Fact]
    public async Task BusinessRuleFailure_ReturnsConflictProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateCourseAsync(instructorId, cancellationToken);

        var response = await this.PostAsRoleAsync(
            $"/api/courses/{courseId}/submit-for-review",
            null,
            instructorId,
            "Instructor",
            cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Conflict,
            "conflict.business_rule",
            "Conflict",
            cancellationToken);
    }

    [Fact]
    public async Task RouteIdMismatch_ReturnsBadRequestProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var routeCourseId = Guid.NewGuid();
        var payloadCourseId = Guid.NewGuid();

        var response = await this.PutAsRoleAsync(
            $"/api/courses/{routeCourseId}",
            new
            {
                CourseId = payloadCourseId,
                Title = "Route mismatch course",
                Description = "A valid payload that should fail before reaching the handler.",
                CategoryId = CourseCategory.Programming.Id,
                LevelId = CourseLevel.Beginner.Id,
                Price = 0m,
                DurationHours = 1,
                DurationMinutes = 0,
                IsFeatured = false,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.BadRequest,
            "request.route_id_mismatch",
            "Invalid request",
            cancellationToken);
    }

    [Fact]
    public async Task MissingToken_ReturnsUnauthorizedProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var response = await this.client.GetAsync($"/api/students/{Guid.NewGuid()}/progress", cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Unauthorized,
            "authentication.unauthorized",
            "Unauthorized",
            cancellationToken);
    }

    [Fact]
    public async Task WrongRole_ReturnsForbiddenProblemDetailsWithStableCode()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/instructors/{Guid.NewGuid()}/pending-submissions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(Guid.NewGuid(), "Student"));

        var response = await this.client.SendAsync(request, cancellationToken);

        using var problem = await AssertProblemAsync(
            response,
            HttpStatusCode.Forbidden,
            "authorization.forbidden",
            "Forbidden",
            cancellationToken);
    }

    private async Task<Guid> CreateCourseAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        var title = $"API error contract {Guid.NewGuid():N}";
        var response = await this.PostAsRoleAsync(
            "/api/courses",
            new
            {
                Title = title,
                Description = "A course created for API error contract tests.",
                CategoryId = CourseCategory.Programming.Id,
                LevelId = CourseLevel.Beginner.Id,
                Price = 0m,
                DurationHours = 1,
                DurationMinutes = 0,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var course = await dbContext.Courses.SingleAsync(course => course.Title == title, cancellationToken);
        return course.Id;
    }

    private async Task<HttpResponseMessage> PostAsRoleAsync(
        string requestUri,
        object? payload,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = payload is null ? null : JsonContent.Create(payload),
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(userId, role));
        return await this.client.SendAsync(request, cancellationToken);
    }

    private async Task<HttpResponseMessage> PutAsRoleAsync(
        string requestUri,
        object payload,
        Guid userId,
        string role,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
        {
            Content = JsonContent.Create(payload),
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CreateJwt(userId, role));
        return await this.client.SendAsync(request, cancellationToken);
    }

    private async Task<Guid> SeedInstructorAsync(CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var uniqueValue = Guid.NewGuid().ToString("N");

        var instructor = new Instructor(
            firstName: "API",
            lastName: "Tester",
            email: Email.Create($"api.error.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"),
            bio: "Validates API error contracts.",
            expertise: "Backend engineering");

        dbContext.Instructors.Add(instructor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return instructor.Id;
    }

    private static async Task<JsonDocument> AssertProblemAsync(
        HttpResponseMessage response,
        HttpStatusCode statusCode,
        string code,
        string title,
        CancellationToken cancellationToken)
    {
        response.StatusCode.Should().Be(statusCode);
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var problem = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);

        problem.RootElement.GetProperty("status").GetInt32().Should().Be((int)statusCode);
        problem.RootElement.GetProperty("title").GetString().Should().Be(title);
        problem.RootElement.GetProperty("code").GetString().Should().Be(code);
        problem.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrWhiteSpace();

        return problem;
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
