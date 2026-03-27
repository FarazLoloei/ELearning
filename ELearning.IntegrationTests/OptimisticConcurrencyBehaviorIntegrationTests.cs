// <copyright file="OptimisticConcurrencyBehaviorIntegrationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.Infrastructure;
using ELearning.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class OptimisticConcurrencyBehaviorIntegrationTests
{
    [Fact]
    public async Task SavingStaleAggregateVersion_ShouldThrowDbUpdateConcurrencyException()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();

        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var instructor = new Instructor("Concurrent", "Instructor", Email.Create($"concurrent.{Guid.NewGuid():N}@tests.io"), "hash");
        var course = new Course(
            "Concurrency Course",
            "Initial description",
            instructor.Id,
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.Create(1, 0),
            10m);

        dbContext.Instructors.Add(instructor);
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync(cancellationToken);

        course.UpdatePrice(20m);

        // Simulate stale version to force optimistic concurrency failure.
        dbContext.Entry(course).Property(nameof(Course.RowVersion)).OriginalValue = new byte[] { 1, 2, 3, 4 };

        var act = async () => await dbContext.SaveChangesAsync(cancellationToken);
        await act.Should().ThrowAsync<DbUpdateConcurrencyException>();
    }
}
