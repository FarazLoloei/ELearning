// <copyright file="OutboxIntegrationEventMapper.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Outbox;

using System.Text.Json;
using ELearning.Infrastructure.Data.Models;

public sealed class OutboxIntegrationEventMapper : IOutboxIntegrationEventMapper
{
    public const string CoursePublishedRoutingKey = "course.published.v1";

    public const string StudentEnrolledRoutingKey = "student.enrolled.v1";

    public const string SubmissionGradedRoutingKey = "submission.graded.v1";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public IntegrationEventPublishMessage? Map(OutboxMessage message)
    {
        if (IsEventType(message.Type, "CoursePublishedEvent"))
        {
            return MapCoursePublished(message);
        }

        if (IsEventType(message.Type, "EnrollmentCreatedEvent"))
        {
            return MapStudentEnrolled(message);
        }

        if (IsEventType(message.Type, "SubmissionGradedEvent"))
        {
            return MapSubmissionGraded(message);
        }

        return null;
    }

    private static IntegrationEventPublishMessage MapCoursePublished(OutboxMessage message)
    {
        var payload = DeserializePayload<CoursePublishedPayload>(message.Payload);
        var integrationEvent = new CoursePublishedIntegrationEvent(
            message.Id,
            payload.CourseId,
            payload.OccurredOnUTC);

        return CreatePublishMessage(
            message,
            nameof(CoursePublishedIntegrationEvent),
            CoursePublishedRoutingKey,
            integrationEvent);
    }

    private static IntegrationEventPublishMessage MapStudentEnrolled(OutboxMessage message)
    {
        var payload = DeserializePayload<EnrollmentCreatedPayload>(message.Payload);
        var integrationEvent = new StudentEnrolledIntegrationEvent(
            message.Id,
            payload.StudentId,
            payload.CourseId,
            payload.EnrollmentId,
            payload.OccurredOnUTC);

        return CreatePublishMessage(
            message,
            nameof(StudentEnrolledIntegrationEvent),
            StudentEnrolledRoutingKey,
            integrationEvent);
    }

    private static IntegrationEventPublishMessage MapSubmissionGraded(OutboxMessage message)
    {
        var payload = DeserializePayload<SubmissionGradedPayload>(message.Payload);
        var integrationEvent = new SubmissionGradedIntegrationEvent(
            message.Id,
            payload.SubmissionId,
            payload.OccurredOnUTC);

        return CreatePublishMessage(
            message,
            nameof(SubmissionGradedIntegrationEvent),
            SubmissionGradedRoutingKey,
            integrationEvent);
    }

    private static IntegrationEventPublishMessage CreatePublishMessage<TEvent>(
        OutboxMessage message,
        string eventType,
        string routingKey,
        TEvent integrationEvent)
    {
        return new IntegrationEventPublishMessage(
            message.Id,
            eventType,
            routingKey,
            JsonSerializer.Serialize(integrationEvent, JsonOptions),
            message.OccurredOnUtc);
    }

    private static TPayload DeserializePayload<TPayload>(string payload)
    {
        return JsonSerializer.Deserialize<TPayload>(payload, JsonOptions)
            ?? throw new InvalidOperationException("Outbox message payload could not be deserialized.");
    }

    private static bool IsEventType(string actualType, string expectedType) =>
        actualType.Equals(expectedType, StringComparison.Ordinal) ||
        actualType.EndsWith($".{expectedType}", StringComparison.Ordinal);

    private sealed record CoursePublishedPayload(Guid CourseId, DateTime OccurredOnUTC);

    private sealed record EnrollmentCreatedPayload(
        Guid StudentId,
        Guid CourseId,
        Guid EnrollmentId,
        DateTime OccurredOnUTC);

    private sealed record SubmissionGradedPayload(Guid SubmissionId, DateTime OccurredOnUTC);
}
