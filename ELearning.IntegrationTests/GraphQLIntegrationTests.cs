// <copyright file="GraphQLIntegrationTests.cs" company="FarazLoloei">
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
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using ELearning.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

public sealed class GraphQLIntegrationTests : IClassFixture<RealAuthWebApplicationFactory>
{
    private const string JwtIssuer = "integration-tests";
    private const string JwtAudience = "integration-tests";
    private const string JwtSecret = "integration-tests-secret-key-with-32chars";

    private readonly RealAuthWebApplicationFactory factory;
    private readonly HttpClient client;

    public GraphQLIntegrationTests(RealAuthWebApplicationFactory factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task PublicCoursesQuery_ReturnsPublishedCoursesShape()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var title = await this.SeedPublishedCourseAsync(cancellationToken);
        var query = $$"""
            query {
              courses(searchTerm: "{{title}}", first: 10) {
                nodes {
                  id
                  title
                  instructorName
                  category
                  level
                }
              }
            }
            """;

        var response = await this.PostGraphQlAsync(query, token: null, cancellationToken);

        using var document = await AssertGraphQlOkAsync(response, cancellationToken);
        var root = document.RootElement;
        AssertNoGraphQlErrors(root);

        var nodes = root
            .GetProperty("data")
            .GetProperty("courses")
            .GetProperty("nodes")
            .EnumerateArray()
            .ToArray();

        nodes.Should().ContainSingle(node =>
            node.GetProperty("title").GetString() == title &&
            !string.IsNullOrWhiteSpace(node.GetProperty("id").GetString()) &&
            node.GetProperty("instructorName").GetString() == "GraphQL Instructor" &&
            node.GetProperty("category").GetString() == CourseCategory.Programming.Name &&
            node.GetProperty("level").GetString() == CourseLevel.Beginner.Name);
    }

    [Fact]
    public async Task ProtectedQueryWithoutToken_ReturnsGraphQlAuthorizationError()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var studentId = Guid.NewGuid();
        var query = $$"""
            query {
              studentProgress(studentId: "{{studentId}}") {
                studentId
                studentName
                completedCourses
                inProgressCourses
              }
            }
            """;

        var response = await this.PostGraphQlAsync(query, token: null, cancellationToken);

        using var document = await AssertGraphQlOkAsync(response, cancellationToken);
        AssertGraphQlAuthorizationError(document.RootElement, "AUTH_NOT_AUTHENTICATED");
    }

    [Fact]
    public async Task InstructorMutationWithStudentToken_ReturnsGraphQlAuthorizationError()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var studentId = await this.SeedStudentAsync(cancellationToken);
        var title = CreateUniqueTitle("wrong-role");
        var query = $$"""
            mutation {
              createCourse(input: {
                title: "{{title}}"
                description: "GraphQL wrong-role mutation should be blocked before resolver execution."
                categoryId: 1
                levelId: 1
                price: 0
                durationHours: 1
                durationMinutes: 0
              }) {
                isSuccess
                errors {
                  code
                  message
                }
              }
            }
            """;

        var response = await this.PostGraphQlAsync(query, CreateJwt(studentId, "Student"), cancellationToken);

        using var document = await AssertGraphQlOkAsync(response, cancellationToken);
        AssertGraphQlAuthorizationError(document.RootElement, "AUTH_NOT_AUTHORIZED");
    }

    [Fact]
    public async Task CreateCourseMutationWithInstructorToken_ReturnsSuccessAndPersistsDraftCourse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var title = CreateUniqueTitle("happy-path");
        var query = $$"""
            mutation {
              createCourse(input: {
                title: "{{title}}"
                description: "GraphQL happy-path mutation smoke test course."
                categoryId: 1
                levelId: 1
                price: 0
                durationHours: 1
                durationMinutes: 0
              }) {
                isSuccess
                errors {
                  code
                  message
                }
              }
            }
            """;

        var response = await this.PostGraphQlAsync(query, CreateJwt(instructorId, "Instructor"), cancellationToken);

        using var document = await AssertGraphQlOkAsync(response, cancellationToken);
        var root = document.RootElement;
        AssertNoGraphQlErrors(root);

        var payload = root
            .GetProperty("data")
            .GetProperty("createCourse");

        payload.GetProperty("isSuccess").GetBoolean().Should().BeTrue();
        payload.GetProperty("errors").EnumerateArray().Should().BeEmpty();

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var course = await dbContext.Courses.SingleAsync(course => course.Title == title, cancellationToken);
        course.InstructorId.Should().Be(instructorId);
        course.Status.Should().Be(CourseStatus.Draft);
    }

    private async Task<HttpResponseMessage> PostGraphQlAsync(
        string query,
        string? token,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/graphql")
        {
            Content = JsonContent.Create(new { query }),
        };

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await this.client.SendAsync(request, cancellationToken);
    }

    private async Task<string> SeedPublishedCourseAsync(CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var uniqueValue = Guid.NewGuid().ToString("N");

        var instructor = new Instructor(
            firstName: "GraphQL",
            lastName: "Instructor",
            email: Email.Create($"graphql.instructor.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"),
            bio: "Builds GraphQL smoke tests.",
            expertise: "Backend engineering");

        var title = CreateUniqueTitle("public-query");
        var course = new Course(
            title,
            "A published course seeded for GraphQL public query coverage.",
            instructor.Id,
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.Create(1, 0),
            0m);

        course.AddModule(new Module(
            "GraphQL module",
            "Module required before publication.",
            1,
            course.Id));
        course.SubmitForReview();
        course.ApprovePublication();

        dbContext.Instructors.Add(instructor);
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync(cancellationToken);

        return title;
    }

    private async Task<Guid> SeedInstructorAsync(CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var uniqueValue = Guid.NewGuid().ToString("N");

        var instructor = new Instructor(
            firstName: "GraphQL",
            lastName: "Instructor",
            email: Email.Create($"graphql.course.author.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"),
            bio: "Builds GraphQL mutation tests.",
            expertise: "Backend engineering");

        dbContext.Instructors.Add(instructor);
        await dbContext.SaveChangesAsync(cancellationToken);
        return instructor.Id;
    }

    private async Task<Guid> SeedStudentAsync(CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var uniqueValue = Guid.NewGuid().ToString("N");

        var student = new Student(
            firstName: "GraphQL",
            lastName: "Student",
            email: Email.Create($"graphql.student.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"));

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync(cancellationToken);
        return student.Id;
    }

    private static async Task<JsonDocument> AssertGraphQlOkAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        response.StatusCode.Should().Be(
            HttpStatusCode.OK,
            await response.Content.ReadAsStringAsync(cancellationToken));

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
    }

    private static void AssertNoGraphQlErrors(JsonElement root)
    {
        root.TryGetProperty("errors", out _).Should().BeFalse(root.GetRawText());
    }

    private static void AssertGraphQlAuthorizationError(JsonElement root, string expectedCode)
    {
        root.TryGetProperty("errors", out var errors).Should().BeTrue(root.GetRawText());
        errors.ValueKind.Should().Be(JsonValueKind.Array);
        errors.GetArrayLength().Should().BeGreaterThan(0);

        var error = errors.EnumerateArray().First();
        error.TryGetProperty("message", out var message).Should().BeTrue(root.GetRawText());
        message.GetString().Should().NotBeNullOrWhiteSpace();

        error.TryGetProperty("extensions", out var extensions).Should().BeTrue(root.GetRawText());
        extensions.TryGetProperty("code", out var code).Should().BeTrue(root.GetRawText());
        code.GetString().Should().Be(expectedCode);
    }

    private static string CreateUniqueTitle(string prefix) =>
        $"GraphQL {prefix} {Guid.NewGuid():N}";

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
