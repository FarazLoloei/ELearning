using ELearning.API.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace ELearning.IntegrationTests;

public sealed class OcelotGatewayModeTests
{
    [Fact]
    public void IsEnabled_WhenFlagMissing_ReturnsFalse()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var enabled = OcelotGatewayMode.IsEnabled(configuration);

        Assert.False(enabled);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void IsEnabled_WhenFlagProvided_ResolvesValue(string rawValue, bool expected)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ocelot:Enabled"] = rawValue
            })
            .Build();

        var enabled = OcelotGatewayMode.IsEnabled(configuration);

        Assert.Equal(expected, enabled);
    }
}
