// <copyright file="LayerDependencyTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Architecture;

using FluentAssertions;
using NetArchTest.Rules;

public sealed class LayerDependencyTests
{
    private const string ApiNamespace = "ELearning.API";
    private const string ApplicationNamespace = "ELearning.Application";
    private const string DomainNamespace = "ELearning.Domain";
    private const string InfrastructureNamespace = "ELearning.Infrastructure";

    [Fact]
    public void Domain_ShouldNotDependOnApplicationApiOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(ELearning.Domain.Entities.UserAggregate.User).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, ApiNamespace, InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Application_ShouldNotDependOnApiOrInfrastructure()
    {
        var result = Types.InAssembly(typeof(ELearning.Application.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace, InfrastructureNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Infrastructure_ShouldNotDependOnApi()
    {
        var result = Types.InAssembly(typeof(ELearning.Infrastructure.DependencyInjection).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Api_ShouldNotDependOnDomain()
    {
        var result = Types.InAssembly(typeof(Program).Assembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace)
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
