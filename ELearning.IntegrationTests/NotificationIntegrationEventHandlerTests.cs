// <copyright file="NotificationIntegrationEventHandlerTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.Text.Json;
using ELearning.Application.Common.Interfaces;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Notifications;
using ELearning.Infrastructure.Outbox;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

public sealed class NotificationIntegrationEventHandlerTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task HandleAsync_ShouldSendEmailAndRecordProcessedMessage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var dbContext = await CreateDbContextAsync(cancellationToken);
        var emailService = new FakeEmailService();
        var handler = CreateHandler(dbContext, emailService);
        var integrationEvent = CreateNotificationRequestedEvent();

        var result = await handler.HandleAsync(
            integrationEvent.EventId,
            nameof(NotificationRequestedIntegrationEvent),
            JsonSerializer.Serialize(integrationEvent, JsonOptions),
            cancellationToken);

        result.Should().Be(NotificationProcessingResult.Processed);
        emailService.SentEmails.Should().ContainSingle();
        dbContext.ProcessedIntegrationMessages.Should().ContainSingle(
            message => message.MessageId == integrationEvent.EventId &&
                       message.Consumer == NotificationIntegrationEventHandler.ConsumerName);
    }

    [Fact]
    public async Task HandleAsync_WhenMessageWasAlreadyProcessed_ShouldNotSendEmailAgain()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var dbContext = await CreateDbContextAsync(cancellationToken);
        var integrationEvent = CreateNotificationRequestedEvent();
        dbContext.ProcessedIntegrationMessages.Add(new ProcessedIntegrationMessage
        {
            MessageId = integrationEvent.EventId,
            Consumer = NotificationIntegrationEventHandler.ConsumerName,
            Type = nameof(NotificationRequestedIntegrationEvent),
            ProcessedOnUtc = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync(cancellationToken);

        var emailService = new FakeEmailService();
        var handler = CreateHandler(dbContext, emailService);

        var result = await handler.HandleAsync(
            integrationEvent.EventId,
            nameof(NotificationRequestedIntegrationEvent),
            JsonSerializer.Serialize(integrationEvent, JsonOptions),
            cancellationToken);

        result.Should().Be(NotificationProcessingResult.AlreadyProcessed);
        emailService.SentEmails.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_WhenEmailFails_ShouldNotRecordProcessedMessage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var dbContext = await CreateDbContextAsync(cancellationToken);
        var emailService = new FakeEmailService(new InvalidOperationException("Email send failed."));
        var handler = CreateHandler(dbContext, emailService);
        var integrationEvent = CreateNotificationRequestedEvent();

        var act = async () => await handler.HandleAsync(
            integrationEvent.EventId,
            nameof(NotificationRequestedIntegrationEvent),
            JsonSerializer.Serialize(integrationEvent, JsonOptions),
            cancellationToken);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email send failed.");
        dbContext.ProcessedIntegrationMessages.Should().BeEmpty();
    }

    private static NotificationIntegrationEventHandler CreateHandler(
        ApplicationDbContext dbContext,
        IEmailService emailService) =>
        new(dbContext, emailService, NullLogger<NotificationIntegrationEventHandler>.Instance);

    private static NotificationRequestedIntegrationEvent CreateNotificationRequestedEvent() =>
        new(
            Guid.NewGuid(),
            "enrollment.confirmation",
            "student@example.com",
            "Student One",
            "Welcome",
            "Hello",
            IsHtml: false,
            "Enrollment",
            Guid.NewGuid(),
            DateTime.UtcNow);

    private static async Task<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync(cancellationToken);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new ApplicationDbContext(options);
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        return dbContext;
    }

    private sealed class FakeEmailService(Exception? exception = null) : IEmailService
    {
        public List<string> SentEmails { get; } = [];

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            if (exception is not null)
            {
                throw exception;
            }

            this.SentEmails.Add(to);
            return Task.CompletedTask;
        }

        public Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName) =>
            Task.CompletedTask;

        public Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score) =>
            Task.CompletedTask;

        public Task SendCourseApprovedAsync(string to, string instructorName, string courseName) =>
            Task.CompletedTask;

        public Task SendCourseRejectedAsync(string to, string instructorName, string courseName, string reason) =>
            Task.CompletedTask;

        public Task SendCertificateIssuedAsync(string to, string studentName, string courseName, string certificateCode) =>
            Task.CompletedTask;
    }
}
