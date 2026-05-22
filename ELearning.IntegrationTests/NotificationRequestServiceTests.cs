// <copyright file="NotificationRequestServiceTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.Text.Json;
using ELearning.Application.Common.Interfaces;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Notifications;
using ELearning.Infrastructure.Options;
using ELearning.Infrastructure.Outbox;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public sealed class NotificationRequestServiceTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task RequestEnrollmentConfirmationAsync_WhenRabbitMqIsDisabled_ShouldSendEmailDirectly()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var dbContext = await CreateDbContextAsync(cancellationToken);
        var emailService = new FakeEmailService();
        var service = new NotificationRequestService(
            dbContext,
            emailService,
            Options.Create(new RabbitMqOptions { Enabled = false }));

        await service.RequestEnrollmentConfirmationAsync(
            "student@example.com",
            "Student One",
            "Clean Architecture",
            Guid.NewGuid(),
            cancellationToken);

        emailService.SentEmails.Should().ContainSingle();
        dbContext.OutboxMessages.Should().BeEmpty();
    }

    [Fact]
    public async Task RequestEnrollmentConfirmationAsync_WhenRabbitMqIsEnabled_ShouldWriteNotificationRequestedOutboxMessage()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var dbContext = await CreateDbContextAsync(cancellationToken);
        var emailService = new FakeEmailService();
        var service = new NotificationRequestService(
            dbContext,
            emailService,
            Options.Create(new RabbitMqOptions { Enabled = true }));

        await service.RequestEnrollmentConfirmationAsync(
            "student@example.com",
            "Student One",
            "Clean Architecture",
            Guid.NewGuid(),
            cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        emailService.SentEmails.Should().BeEmpty();
        var outboxMessage = dbContext.OutboxMessages.Should().ContainSingle().Subject;
        outboxMessage.Type.Should().Be(nameof(NotificationRequestedIntegrationEvent));

        var integrationEvent = JsonSerializer.Deserialize<NotificationRequestedIntegrationEvent>(
            outboxMessage.Payload,
            JsonOptions);
        integrationEvent.Should().NotBeNull();
        integrationEvent!.RecipientEmail.Should().Be("student@example.com");
        integrationEvent.NotificationType.Should().Be("enrollment.confirmation");

        var publishMessage = new OutboxIntegrationEventMapper().Map(outboxMessage);
        publishMessage.Should().NotBeNull();
        publishMessage!.RoutingKey.Should().Be(OutboxIntegrationEventMapper.NotificationRequestedRoutingKey);
    }

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

    private sealed class FakeEmailService : IEmailService
    {
        public List<string> SentEmails { get; } = [];

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = false)
        {
            this.SentEmails.Add(to);
            return Task.CompletedTask;
        }

        public Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName)
        {
            this.SentEmails.Add(to);
            return Task.CompletedTask;
        }

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
