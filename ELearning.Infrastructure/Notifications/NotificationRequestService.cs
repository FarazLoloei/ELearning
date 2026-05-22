// <copyright file="NotificationRequestService.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Notifications;

using System.Text.Json;
using ELearning.Application.Common.Interfaces;
using ELearning.Infrastructure.Data;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Options;
using ELearning.Infrastructure.Outbox;
using Microsoft.Extensions.Options;

public sealed class NotificationRequestService(
    ApplicationDbContext dbContext,
    IEmailService emailService,
    IOptions<RabbitMqOptions> rabbitMqOptions) : INotificationRequestService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RabbitMqOptions options = rabbitMqOptions.Value;

    public Task RequestEnrollmentConfirmationAsync(
        string recipientEmail,
        string studentName,
        string courseName,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var subject = $"Welcome to {courseName}";
        var body = $"Dear {studentName},\n\nThank you for enrolling in {courseName}. We hope you enjoy the course!\n\nBest regards,\nE-Learning Team";

        return this.RequestAsync(
            notificationType: "enrollment.confirmation",
            recipientEmail,
            recipientName: studentName,
            subject,
            body,
            source: "Enrollment",
            sourceId,
            fallback: () => emailService.SendEnrollmentConfirmationAsync(recipientEmail, studentName, courseName),
            cancellationToken);
    }

    public Task RequestAssignmentGradedAsync(
        string recipientEmail,
        string studentName,
        string assignmentName,
        int score,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var subject = $"Your assignment {assignmentName} has been graded";
        var body = $"Dear {studentName},\n\nYour assignment {assignmentName} has been graded. You received {score} points.\n\nBest regards,\nE-Learning Team";

        return this.RequestAsync(
            notificationType: "assignment.graded",
            recipientEmail,
            recipientName: studentName,
            subject,
            body,
            source: "Submission",
            sourceId,
            fallback: () => emailService.SendAssignmentGradedAsync(recipientEmail, studentName, assignmentName, score),
            cancellationToken);
    }

    public Task RequestCourseApprovedAsync(
        string recipientEmail,
        string instructorName,
        string courseName,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var subject = $"{courseName} is now published";
        var body = $"Dear {instructorName},\n\nYour course {courseName} has been approved and published.\n\nBest regards,\nE-Learning Team";

        return this.RequestAsync(
            notificationType: "course.approved",
            recipientEmail,
            recipientName: instructorName,
            subject,
            body,
            source: "Course",
            sourceId,
            fallback: () => emailService.SendCourseApprovedAsync(recipientEmail, instructorName, courseName),
            cancellationToken);
    }

    public Task RequestCourseRejectedAsync(
        string recipientEmail,
        string instructorName,
        string courseName,
        string reason,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var subject = $"{courseName} needs changes before publication";
        var body = $"Dear {instructorName},\n\nYour course {courseName} was rejected during review.\nReason: {reason}\n\nYou can update the course and resubmit it for review.\n\nBest regards,\nE-Learning Team";

        return this.RequestAsync(
            notificationType: "course.rejected",
            recipientEmail,
            recipientName: instructorName,
            subject,
            body,
            source: "Course",
            sourceId,
            fallback: () => emailService.SendCourseRejectedAsync(recipientEmail, instructorName, courseName, reason),
            cancellationToken);
    }

    public Task RequestCertificateIssuedAsync(
        string recipientEmail,
        string studentName,
        string courseName,
        string certificateCode,
        Guid sourceId,
        CancellationToken cancellationToken)
    {
        var subject = $"Your certificate for {courseName} is ready";
        var body = $"Dear {studentName},\n\nCongratulations on completing {courseName}.\nYour certificate code is {certificateCode}.\n\nBest regards,\nE-Learning Team";

        return this.RequestAsync(
            notificationType: "certificate.issued",
            recipientEmail,
            recipientName: studentName,
            subject,
            body,
            source: "Certificate",
            sourceId,
            fallback: () => emailService.SendCertificateIssuedAsync(recipientEmail, studentName, courseName, certificateCode),
            cancellationToken);
    }

    private Task RequestAsync(
        string notificationType,
        string recipientEmail,
        string recipientName,
        string subject,
        string body,
        string source,
        Guid sourceId,
        Func<Task> fallback,
        CancellationToken cancellationToken)
    {
        if (!this.options.Enabled)
        {
            return fallback();
        }

        var integrationEvent = new NotificationRequestedIntegrationEvent(
            Guid.NewGuid(),
            notificationType,
            recipientEmail,
            recipientName,
            subject,
            body,
            IsHtml: false,
            source,
            sourceId,
            DateTime.UtcNow);

        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            Id = integrationEvent.EventId,
            OccurredOnUtc = integrationEvent.OccurredOnUtc,
            Type = nameof(NotificationRequestedIntegrationEvent),
            Payload = JsonSerializer.Serialize(integrationEvent, JsonOptions),
        });

        return Task.CompletedTask;
    }
}
