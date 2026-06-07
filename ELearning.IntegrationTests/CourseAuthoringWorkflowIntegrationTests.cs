// <copyright file="CourseAuthoringWorkflowIntegrationTests.cs" company="FarazLoloei">
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

public sealed class CourseAuthoringWorkflowIntegrationTests : IClassFixture<RealAuthWebApplicationFactory>
{
    private const string JwtIssuer = "integration-tests";
    private const string JwtAudience = "integration-tests";
    private const string JwtSecret = "integration-tests-secret-key-with-32chars";

    private readonly RealAuthWebApplicationFactory factory;
    private readonly HttpClient client;

    public CourseAuthoringWorkflowIntegrationTests(RealAuthWebApplicationFactory factory)
    {
        this.factory = factory;
        this.client = factory.CreateClient();
    }

    [Fact]
    public async Task InstructorCreatesCourse_ReturnsCreatedAndPersistsDraftCourse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var title = CreateUniqueTitle("create-course");

        var response = await this.PostAsRoleAsync(
            "/api/v1/courses",
            CreateCourseRequest(title),
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCourseId = await ReadResponseDataIdAsync(response, cancellationToken);
        var course = await this.GetCourseByTitleAsync(title, cancellationToken);
        createdCourseId.Should().Be(course.Id);
        course.InstructorId.Should().Be(instructorId);
        course.Status.Should().Be(CourseStatus.Draft);
    }

    [Fact]
    public async Task GetInstructorWithCourses_ReturnsCreatedDraftCourse()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var title = CreateUniqueTitle("with-courses");
        var courseId = await this.CreateCourseAsync(instructorId, title, cancellationToken);

        var response = await this.client.GetAsync($"/api/instructors/{instructorId}/with-courses", cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var content = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        content.RootElement.GetProperty("succeeded").GetBoolean().Should().BeTrue();

        var courses = content.RootElement.GetProperty("data").GetProperty("courses");
        courses.EnumerateArray()
            .Any(course => course.GetProperty("id").GetGuid() == courseId && course.GetProperty("title").GetString() == title)
            .Should()
            .BeTrue();
    }

    [Fact]
    public async Task CourseCannotBeSubmittedWithoutContent_ReturnsConflict()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateCourseAsync(instructorId, CreateUniqueTitle("submit-empty"), cancellationToken);

        var response = await this.PostAsRoleAsync(
            $"/api/v1/courses/{courseId}/submit-for-review",
            null,
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task InstructorAddsModule_ReturnsCreatedAndPersistsModule()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateCourseAsync(instructorId, CreateUniqueTitle("add-module"), cancellationToken);

        var moduleId = await this.AddModuleAsync(courseId, instructorId, cancellationToken);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var module = await dbContext.Modules.SingleAsync(module => module.Id == moduleId, cancellationToken);
        module.CourseId.Should().Be(courseId);
        module.Title.Should().Be("Module 1");
    }

    [Fact]
    public async Task InstructorAddsLesson_ReturnsCreatedAndPersistsLesson()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateCourseAsync(instructorId, CreateUniqueTitle("add-lesson"), cancellationToken);
        var moduleId = await this.AddModuleAsync(courseId, instructorId, cancellationToken);

        var lessonId = await this.AddLessonAsync(courseId, moduleId, instructorId, cancellationToken);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var lesson = await dbContext.Lessons.SingleAsync(lesson => lesson.Id == lessonId, cancellationToken);
        lesson.ModuleId.Should().Be(moduleId);
        lesson.Title.Should().Be("Lesson 1");
        lesson.Type.Should().Be(LessonType.Text);
    }

    [Fact]
    public async Task InstructorAddsAssignment_ReturnsCreatedAndPersistsAssignment()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateCourseAsync(instructorId, CreateUniqueTitle("add-assignment"), cancellationToken);
        var moduleId = await this.AddModuleAsync(courseId, instructorId, cancellationToken);

        var assignmentId = await this.AddAssignmentAsync(courseId, moduleId, instructorId, cancellationToken);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var assignment = await dbContext.Assignments.SingleAsync(assignment => assignment.Id == assignmentId, cancellationToken);
        assignment.ModuleId.Should().Be(moduleId);
        assignment.Title.Should().Be("Assignment 1");
        assignment.Type.Should().Be(AssignmentType.Quiz);
    }

    [Fact]
    public async Task CourseCanBeSubmittedAfterAuthoringContent()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var courseId = await this.CreateAuthoredCourseAsync(instructorId, CreateUniqueTitle("submit-authored"), cancellationToken);

        var response = await this.PostAsRoleAsync(
            $"/api/courses/{courseId}/submit-for-review",
            null,
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var course = await this.GetCourseByIdAsync(courseId, cancellationToken);
        course.Status.Should().Be(CourseStatus.ReadyForReview);
    }

    [Fact]
    public async Task AdminCanApproveOrRejectSubmittedCoursesAfterAuthoring()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var adminId = Guid.NewGuid();
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var approvalCourseId = await this.CreateSubmittedCourseAsync(instructorId, CreateUniqueTitle("approve"), cancellationToken);
        var rejectionCourseId = await this.CreateSubmittedCourseAsync(instructorId, CreateUniqueTitle("reject"), cancellationToken);

        var approveResponse = await this.PostAsRoleAsync(
            $"/api/v1/courses/{approvalCourseId}/approve-publication",
            null,
            adminId,
            "Admin",
            cancellationToken);

        var rejectResponse = await this.PostAsRoleAsync(
            $"/api/v1/courses/{rejectionCourseId}/reject-publication",
            new
            {
                CourseId = rejectionCourseId,
                Reason = "Add a clearer module introduction before publishing.",
            },
            adminId,
            "Admin",
            cancellationToken);

        approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        rejectResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var approvedCourse = await this.GetCourseByIdAsync(approvalCourseId, cancellationToken);
        var rejectedCourse = await this.GetCourseByIdAsync(rejectionCourseId, cancellationToken);
        approvedCourse.Status.Should().Be(CourseStatus.Published);
        rejectedCourse.Status.Should().Be(CourseStatus.Rejected);
        rejectedCourse.RejectionReason.Should().Be("Add a clearer module introduction before publishing.");
    }

    [Fact]
    public async Task StudentCreatesEnrollment_ReturnsCreatedIdAndPersistsEnrollment()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var studentId = await this.SeedStudentAsync(cancellationToken);
        var courseId = await this.CreatePublishedCourseAsync(instructorId, CreateUniqueTitle("enroll"), cancellationToken);

        var enrollmentId = await this.CreateEnrollmentAsync(studentId, courseId, cancellationToken);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var enrollment = await dbContext.Enrollments.SingleAsync(e => e.Id == enrollmentId, cancellationToken);
        enrollment.StudentId.Should().Be(studentId);
        enrollment.CourseId.Should().Be(courseId);
    }

    [Fact]
    public async Task StudentCreatesSubmission_ReturnsCreatedIdAndPersistsSubmission()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var instructorId = await this.SeedInstructorAsync(cancellationToken);
        var studentId = await this.SeedStudentAsync(cancellationToken);
        var courseId = await this.CreatePublishedCourseWithAssignmentAsync(
            instructorId,
            CreateUniqueTitle("submission"),
            cancellationToken);
        var assignmentId = await this.GetFirstAssignmentIdAsync(courseId, cancellationToken);
        var enrollmentId = await this.CreateEnrollmentAsync(studentId, courseId, cancellationToken);

        var submissionId = await this.CreateSubmissionAsync(studentId, assignmentId, cancellationToken);

        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var submission = await dbContext.Submissions.SingleAsync(s => s.Id == submissionId, cancellationToken);
        submission.EnrollmentId.Should().Be(enrollmentId);
        submission.AssignmentId.Should().Be(assignmentId);
    }

    private async Task<Guid> CreateAuthoredCourseAsync(Guid instructorId, string title, CancellationToken cancellationToken)
    {
        var courseId = await this.CreateCourseAsync(instructorId, title, cancellationToken);
        var moduleId = await this.AddModuleAsync(courseId, instructorId, cancellationToken);
        await this.AddLessonAsync(courseId, moduleId, instructorId, cancellationToken);
        return courseId;
    }

    private async Task<Guid> CreateSubmittedCourseAsync(Guid instructorId, string title, CancellationToken cancellationToken)
    {
        var courseId = await this.CreateAuthoredCourseAsync(instructorId, title, cancellationToken);
        var response = await this.PostAsRoleAsync(
            $"/api/v1/courses/{courseId}/submit-for-review",
            null,
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return courseId;
    }

    private async Task<Guid> CreatePublishedCourseAsync(Guid instructorId, string title, CancellationToken cancellationToken)
    {
        var courseId = await this.CreateSubmittedCourseAsync(instructorId, title, cancellationToken);
        var response = await this.PostAsRoleAsync(
            $"/api/v1/courses/{courseId}/approve-publication",
            null,
            Guid.NewGuid(),
            "Admin",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        return courseId;
    }

    private async Task<Guid> CreatePublishedCourseWithAssignmentAsync(Guid instructorId, string title, CancellationToken cancellationToken)
    {
        var courseId = await this.CreateCourseAsync(instructorId, title, cancellationToken);
        var moduleId = await this.AddModuleAsync(courseId, instructorId, cancellationToken);
        await this.AddLessonAsync(courseId, moduleId, instructorId, cancellationToken);
        await this.AddAssignmentAsync(courseId, moduleId, instructorId, cancellationToken);
        var submitResponse = await this.PostAsRoleAsync(
            $"/api/v1/courses/{courseId}/submit-for-review",
            null,
            instructorId,
            "Instructor",
            cancellationToken);
        submitResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var approveResponse = await this.PostAsRoleAsync(
            $"/api/v1/courses/{courseId}/approve-publication",
            null,
            Guid.NewGuid(),
            "Admin",
            cancellationToken);
        approveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        return courseId;
    }

    private async Task<Guid> CreateCourseAsync(Guid instructorId, string title, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            "/api/v1/courses",
            CreateCourseRequest(title),
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var courseId = await ReadResponseDataIdAsync(response, cancellationToken);
        var course = await this.GetCourseByTitleAsync(title, cancellationToken);
        courseId.Should().Be(course.Id);
        course.RowVersion.Should().NotBeNull();
        return courseId;
    }

    private async Task<Guid> AddModuleAsync(Guid courseId, Guid instructorId, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            $"/api/courses/{courseId}/modules",
            new
            {
                Title = "Module 1",
                Description = "Introduction module.",
                Order = 1,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(
            HttpStatusCode.Created,
            await response.Content.ReadAsStringAsync(cancellationToken));
        return await ReadResponseDataIdAsync(response, cancellationToken);
    }

    private async Task<Guid> AddLessonAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            $"/api/courses/{courseId}/modules/{moduleId}/lessons",
            new
            {
                Title = "Lesson 1",
                Content = "Read this introduction.",
                TypeId = LessonType.Text.Id,
                Order = 1,
                DurationHours = 0,
                DurationMinutes = 15,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await ReadResponseDataIdAsync(response, cancellationToken);
    }

    private async Task<Guid> AddAssignmentAsync(Guid courseId, Guid moduleId, Guid instructorId, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            $"/api/courses/{courseId}/modules/{moduleId}/assignments",
            new
            {
                Title = "Assignment 1",
                Description = "Complete the introduction check.",
                TypeId = AssignmentType.Quiz.Id,
                MaxPoints = 10,
            },
            instructorId,
            "Instructor",
            cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return await ReadResponseDataIdAsync(response, cancellationToken);
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

    private async Task<Guid> SeedInstructorAsync(CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var uniqueValue = Guid.NewGuid().ToString("N");

        var instructor = new Instructor(
            firstName: "Integration",
            lastName: "Instructor",
            email: Email.Create($"course.author.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"),
            bio: "Builds course authoring tests.",
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
            firstName: "Integration",
            lastName: "Student",
            email: Email.Create($"course.student.{uniqueValue}@tests.io"),
            passwordHash: passwordHasher.HashPassword("P@ssword123!"));

        dbContext.Students.Add(student);
        await dbContext.SaveChangesAsync(cancellationToken);
        return student.Id;
    }

    private async Task<Course> GetCourseByTitleAsync(
        string title,
        CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Courses
            .SingleAsync(course => course.Title == title, cancellationToken);
    }

    private async Task<Course> GetCourseByIdAsync(
        Guid courseId,
        CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await dbContext.Courses
            .SingleAsync(course => course.Id == courseId, cancellationToken);
    }

    private async Task<Guid> GetFirstAssignmentIdAsync(Guid courseId, CancellationToken cancellationToken)
    {
        using var scope = this.factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var moduleIds = await dbContext.Modules
            .Where(module => module.CourseId == courseId)
            .Select(module => module.Id)
            .ToListAsync(cancellationToken);

        return await dbContext.Assignments
            .Where(assignment => moduleIds.Contains(assignment.ModuleId))
            .Select(assignment => assignment.Id)
            .SingleAsync(cancellationToken);
    }

    private async Task<Guid> CreateEnrollmentAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            "/api/v1/enrollments",
            new
            {
                CourseId = courseId,
            },
            studentId,
            "Student",
            cancellationToken);

        response.StatusCode.Should().Be(
            HttpStatusCode.Created,
            await response.Content.ReadAsStringAsync(cancellationToken));
        return await ReadResponseDataIdAsync(response, cancellationToken);
    }

    private async Task<Guid> CreateSubmissionAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken)
    {
        var response = await this.PostAsRoleAsync(
            "/api/v1/submissions",
            new
            {
                AssignmentId = assignmentId,
                Content = "Integration test submission content.",
                FileUrl = string.Empty,
            },
            studentId,
            "Student",
            cancellationToken);

        response.StatusCode.Should().Be(
            HttpStatusCode.Created,
            await response.Content.ReadAsStringAsync(cancellationToken));
        return await ReadResponseDataIdAsync(response, cancellationToken);
    }

    private static object CreateCourseRequest(string title) => new
    {
        Title = title,
        Description = "A course created by the authoring workflow integration tests.",
        CategoryId = CourseCategory.Programming.Id,
        LevelId = CourseLevel.Beginner.Id,
        Price = 0m,
        DurationHours = 1,
        DurationMinutes = 0,
    };

    private static string CreateUniqueTitle(string prefix) =>
        $"Authoring {prefix} {Guid.NewGuid():N}";

    private static async Task<Guid> ReadResponseDataIdAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var content = await JsonDocument.ParseAsync(responseStream, cancellationToken: cancellationToken);
        return content.RootElement.GetProperty("data").GetGuid();
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
