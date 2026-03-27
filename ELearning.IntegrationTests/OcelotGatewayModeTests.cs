// <copyright file="OcelotGatewayModeTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using ELearning.API.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

public sealed class OcelotGatewayModeTests
{
    [Fact]
    public void IsEnabled_WhenFlagMissing_ReturnsFalse()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var enabled = OcelotGatewayMode.IsEnabled(configuration);

        enabled.Should().BeFalse();
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void IsEnabled_WhenFlagProvided_ResolvesValue(string rawValue, bool expected)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ocelot:Enabled"] = rawValue,
            })
            .Build();

        var enabled = OcelotGatewayMode.IsEnabled(configuration);

        enabled.Should().Be(expected);
    }
}
