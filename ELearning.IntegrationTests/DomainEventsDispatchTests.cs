using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure.Data;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ELearning.IntegrationTests;

public sealed class DomainEventsDispatchTests
{
    [Fact]
    public async Task SaveChangesAsync_WithPublisher_PublishesAndClearsDomainEvents()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var publisher = new RecordingPublisher();
        await using var dbContext = new ApplicationDbContext(options, publisher);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var instructor = CreateInstructor();
        dbContext.Instructors.Add(instructor);
        var course = CreateCourse(instructor.Id);
        dbContext.Courses.Add(course);

        await dbContext.SaveChangesAsync(cancellationToken);

        Assert.Single(publisher.PublishedNotifications);
        Assert.IsType<CourseCreatedEvent>(publisher.PublishedNotifications[0]);
        Assert.Empty(course.DomainEvents);
    }

    [Fact]
    public async Task SaveChangesAsync_WithoutPublisher_ClearsDomainEvents()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var dbContext = new ApplicationDbContext(options, publisher: null);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var instructor = CreateInstructor();
        dbContext.Instructors.Add(instructor);
        var course = CreateCourse(instructor.Id);
        dbContext.Courses.Add(course);

        await dbContext.SaveChangesAsync(cancellationToken);

        Assert.Empty(course.DomainEvents);
    }

    private static Course CreateCourse(Guid instructorId) =>
        new(
            "Domain Event Test Course",
            "Tests domain event dispatch from ApplicationDbContext.",
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

    private sealed class RecordingPublisher : IPublisher
    {
        public List<object> PublishedNotifications { get; } = [];

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            PublishedNotifications.Add(notification);
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            PublishedNotifications.Add(notification!);
            return Task.CompletedTask;
        }
    }
}
