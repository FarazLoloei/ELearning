using ELearning.API.Infrastructure;
using ELearning.Infrastructure;
using ELearning.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ELearning.IntegrationTests;

public sealed class DatabaseInitializerTests
{
    [Theory]
    [InlineData("SqlServer", true)]
    [InlineData("sqlserver", true)]
    [InlineData("SqliteInMemory", false)]
    [InlineData("UnknownProvider", false)]
    [InlineData(null, false)]
    public void ShouldApplyMigrations_UsesExpectedStrategy(string? provider, bool expected)
    {
        var result = DatabaseInitializer.ShouldApplyMigrations(provider);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task InitializeAsync_WithSqliteInMemoryProvider_CreatesDatabase()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = "SqliteInMemory",
                ["Database:SqliteInMemoryConnection"] = "Data Source=:memory:;Cache=Shared"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        var serviceProvider = services.BuildServiceProvider();

        await DatabaseInitializer.InitializeAsync(serviceProvider, configuration, cancellationToken);

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
        Assert.True(canConnect);
    }
}
