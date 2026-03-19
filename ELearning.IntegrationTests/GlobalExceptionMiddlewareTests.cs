// <copyright file="GlobalExceptionMiddlewareTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.Text.Json;
using ELearning.API.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WhenDbUpdateConcurrencyExceptionIsThrown_ReturnsConflictProblemDetails()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionMiddleware(
            _ => throw new DbUpdateConcurrencyException("Concurrency update conflict."),
            NullLogger<GlobalExceptionMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        context.Response.ContentType.Should().StartWith("application/problem+json");

        context.Response.Body.Position = 0;
        using var payload = await JsonDocument.ParseAsync(context.Response.Body, cancellationToken: cancellationToken);
        payload.RootElement.GetProperty("status").GetInt32().Should().Be(StatusCodes.Status409Conflict);
        payload.RootElement.GetProperty("title").GetString().Should().Be("Concurrency conflict");
    }
}
