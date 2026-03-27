// <copyright file="OptimisticConcurrencyConfigurationTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Infrastructure;
using ELearning.Infrastructure.Data;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public sealed class OptimisticConcurrencyConfigurationTests
{
    [Fact]
    public void KeyAggregates_ShouldUseRowVersionConcurrencyToken()
    {
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

        AssertRowVersionConfigured<User>(dbContext);
        AssertRowVersionConfigured<Course>(dbContext);
        AssertRowVersionConfigured<Enrollment>(dbContext);
    }

    private static void AssertRowVersionConfigured<TEntity>(DbContext dbContext)
        where TEntity : class
    {
        var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
        entityType.Should().NotBeNull();

        var rowVersion = entityType!.FindProperty("RowVersion");
        rowVersion.Should().NotBeNull();
        rowVersion!.IsConcurrencyToken.Should().BeTrue();
        rowVersion.ValueGenerated.Should().Be(ValueGenerated.OnAddOrUpdate);
    }
}
