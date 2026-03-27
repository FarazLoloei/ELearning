// <copyright file="CourseLifecycleTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Domain;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using FluentAssertions;

public sealed class CourseLifecycleTests
{
    [Fact]
    public void SubmitForReview_WhenCourseHasNoModules_ThrowsInvalidOperationException()
    {
        var course = CreateCourse();

        var action = () => course.SubmitForReview();

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*at least one module*");
    }

    [Fact]
    public void SubmitForReview_WhenDraftCourseHasStructure_MovesCourseToReadyForReview()
    {
        var course = CreateCourse();
        course.AddModule(new Module("Module 1", "Introduction module.", 1, course.Id));

        course.SubmitForReview();

        course.Status.Should().Be(CourseStatus.ReadyForReview);
        course.RejectionReason.Should().BeNull();
    }

    [Fact]
    public void RejectPublication_WhenCourseIsReadyForReview_MovesCourseToRejected()
    {
        var course = CreateReadyForReviewCourse();

        course.RejectPublication("Add more instructional detail before publishing.");

        course.Status.Should().Be(CourseStatus.Rejected);
        course.RejectionReason.Should().Be("Add more instructional detail before publishing.");
    }

    [Fact]
    public void ApprovePublication_WhenCourseIsReadyForReview_PublishesCourse()
    {
        var course = CreateReadyForReviewCourse();

        course.ApprovePublication();

        course.Status.Should().Be(CourseStatus.Published);
        course.PublishedDate.Should().NotBeNull();
        course.CanAcceptNewEnrollments().Should().BeTrue();
    }

    [Fact]
    public void UpdateDetails_WhenCourseIsPublished_ThrowsInvalidOperationException()
    {
        var course = CreateReadyForReviewCourse();
        course.ApprovePublication();

        var action = () => course.UpdateDetails(
            "Updated title",
            "Updated description",
            CourseCategory.Programming,
            CourseLevel.Intermediate,
            Duration.Create(2, 0));

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*draft or rejected*");
    }

    private static Course CreateCourse()
    {
        return new Course(
            "Lifecycle Test Course",
            "Course used for lifecycle tests.",
            Guid.NewGuid(),
            CourseCategory.Programming,
            CourseLevel.Beginner,
            Duration.Create(1, 0),
            0m);
    }

    private static Course CreateReadyForReviewCourse()
    {
        var course = CreateCourse();
        course.AddModule(new Module("Module 1", "Introduction module.", 1, course.Id));
        course.SubmitForReview();
        return course;
    }
}
