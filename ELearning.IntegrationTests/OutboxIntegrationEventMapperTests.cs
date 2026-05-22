// <copyright file="OutboxIntegrationEventMapperTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.IntegrationTests;

using System.Text.Json;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Infrastructure.Data.Models;
using ELearning.Infrastructure.Outbox;
using FluentAssertions;

public sealed class OutboxIntegrationEventMapperTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public void Map_ShouldCreateCoursePublishedIntegrationEvent()
    {
        var mapper = new OutboxIntegrationEventMapper();
        var messageId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var occurredOnUtc = DateTime.UtcNow;
        var message = CreateMessage(
            messageId,
            typeof(CoursePublishedEvent).FullName!,
            new { CourseId = courseId, OccurredOnUTC = occurredOnUtc });

        var publishMessage = mapper.Map(message);

        publishMessage.Should().NotBeNull();
        publishMessage!.MessageId.Should().Be(messageId);
        publishMessage.EventType.Should().Be(nameof(CoursePublishedIntegrationEvent));
        publishMessage.RoutingKey.Should().Be(OutboxIntegrationEventMapper.CoursePublishedRoutingKey);

        var integrationEvent = JsonSerializer.Deserialize<CoursePublishedIntegrationEvent>(
            publishMessage.Payload,
            JsonOptions);
        integrationEvent.Should().NotBeNull();
        integrationEvent!.EventId.Should().Be(messageId);
        integrationEvent.CourseId.Should().Be(courseId);
        integrationEvent.OccurredOnUtc.Should().BeCloseTo(occurredOnUtc, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Map_ShouldCreateStudentEnrolledIntegrationEvent()
    {
        var mapper = new OutboxIntegrationEventMapper();
        var messageId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var occurredOnUtc = DateTime.UtcNow;
        var message = CreateMessage(
            messageId,
            typeof(EnrollmentCreatedEvent).FullName!,
            new
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentId = enrollmentId,
                OccurredOnUTC = occurredOnUtc,
            });

        var publishMessage = mapper.Map(message);

        publishMessage.Should().NotBeNull();
        publishMessage!.EventType.Should().Be(nameof(StudentEnrolledIntegrationEvent));
        publishMessage.RoutingKey.Should().Be(OutboxIntegrationEventMapper.StudentEnrolledRoutingKey);

        var integrationEvent = JsonSerializer.Deserialize<StudentEnrolledIntegrationEvent>(
            publishMessage.Payload,
            JsonOptions);
        integrationEvent.Should().NotBeNull();
        integrationEvent!.EventId.Should().Be(messageId);
        integrationEvent.StudentId.Should().Be(studentId);
        integrationEvent.CourseId.Should().Be(courseId);
        integrationEvent.EnrollmentId.Should().Be(enrollmentId);
    }

    [Fact]
    public void Map_ShouldCreateSubmissionGradedIntegrationEvent()
    {
        var mapper = new OutboxIntegrationEventMapper();
        var messageId = Guid.NewGuid();
        var submissionId = Guid.NewGuid();
        var occurredOnUtc = DateTime.UtcNow;
        var message = CreateMessage(
            messageId,
            typeof(SubmissionGradedEvent).FullName!,
            new { SubmissionId = submissionId, OccurredOnUTC = occurredOnUtc });

        var publishMessage = mapper.Map(message);

        publishMessage.Should().NotBeNull();
        publishMessage!.EventType.Should().Be(nameof(SubmissionGradedIntegrationEvent));
        publishMessage.RoutingKey.Should().Be(OutboxIntegrationEventMapper.SubmissionGradedRoutingKey);

        var integrationEvent = JsonSerializer.Deserialize<SubmissionGradedIntegrationEvent>(
            publishMessage.Payload,
            JsonOptions);
        integrationEvent.Should().NotBeNull();
        integrationEvent!.EventId.Should().Be(messageId);
        integrationEvent.SubmissionId.Should().Be(submissionId);
    }

    [Fact]
    public void Map_ShouldReturnNullForUnsupportedDomainEvent()
    {
        var mapper = new OutboxIntegrationEventMapper();
        var message = CreateMessage(
            Guid.NewGuid(),
            "ELearning.Domain.Entities.CourseAggregate.Events.CourseCreatedEvent",
            new { CourseId = Guid.NewGuid(), OccurredOnUTC = DateTime.UtcNow });

        var publishMessage = mapper.Map(message);

        publishMessage.Should().BeNull();
    }

    [Fact]
    public void Map_ShouldCreateNotificationRequestedIntegrationEvent()
    {
        var mapper = new OutboxIntegrationEventMapper();
        var integrationEvent = new NotificationRequestedIntegrationEvent(
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
        var message = CreateMessage(
            integrationEvent.EventId,
            nameof(NotificationRequestedIntegrationEvent),
            integrationEvent);

        var publishMessage = mapper.Map(message);

        publishMessage.Should().NotBeNull();
        publishMessage!.EventType.Should().Be(nameof(NotificationRequestedIntegrationEvent));
        publishMessage.RoutingKey.Should().Be(OutboxIntegrationEventMapper.NotificationRequestedRoutingKey);
    }

    private static OutboxMessage CreateMessage(Guid id, string type, object payload) =>
        new()
        {
            Id = id,
            Type = type,
            Payload = JsonSerializer.Serialize(payload, JsonOptions),
            OccurredOnUtc = DateTime.UtcNow,
        };
}
