// <copyright file="IntegrationEvents.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

public sealed record CoursePublishedIntegrationEvent(
    Guid EventId,
    Guid CourseId,
    DateTime OccurredOnUtc);

public sealed record StudentEnrolledIntegrationEvent(
    Guid EventId,
    Guid StudentId,
    Guid CourseId,
    Guid EnrollmentId,
    DateTime OccurredOnUtc);

public sealed record SubmissionGradedIntegrationEvent(
    Guid EventId,
    Guid SubmissionId,
    DateTime OccurredOnUtc);

public sealed record NotificationRequestedIntegrationEvent(
    Guid EventId,
    string NotificationType,
    string RecipientEmail,
    string RecipientName,
    string Subject,
    string Body,
    bool IsHtml,
    string Source,
    Guid SourceId,
    DateTime OccurredOnUtc);
