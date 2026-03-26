// <copyright file="DomainEventsDispatchTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public sealed class DomainEventsDispatchTests
{
    [Fact]
    public async Task SaveChangesAsync_ShouldWriteDomainEventsToOutbox_AndClearDomainEvents()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var instructor = CreateInstructor();
        dbContext.Instructors.Add(instructor);
        var course = CreateCourse(instructor.Id);
        dbContext.Courses.Add(course);

        await dbContext.SaveChangesAsync(cancellationToken);

        course.DomainEvents.Should().BeEmpty();

        var outboxMessages = await dbContext.OutboxMessages.ToListAsync(cancellationToken);
        outboxMessages.Should().ContainSingle();
        outboxMessages[0].Type.Should().Contain("CourseCreatedEvent");
        outboxMessages[0].ProcessedOnUtc.Should().BeNull();
    }

    private static Course CreateCourse(Guid instructorId) =>
        new(
            "Domain Event Test Course",
            "Tests domain event outbox from ApplicationDbContext.",
            instructorId,
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.Create(1, 0),
            0m);

    private static Instructor CreateInstructor() =>
        new(
            "Domain",
            "Publisher",
            Email.Create("domain.publisher@tests.io"),
            "hashed-password");
}
