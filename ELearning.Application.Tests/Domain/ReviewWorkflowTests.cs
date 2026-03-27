// <copyright file="ReviewWorkflowTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Domain;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.ValueObjects;
using FluentAssertions;

public sealed class ReviewWorkflowTests
{
    [Fact]
    public void ReviewCourse_WhenEnrollmentIsCompleted_StoresRatingReviewAndReviewedDate()
    {
        var enrollment = CreateCompletedEnrollment();

        enrollment.ReviewCourse(5, "Excellent course.");

        enrollment.CourseRating.Should().NotBeNull();
        enrollment.CourseRating!.Value.Should().Be(5);
        enrollment.CourseRating.NumberOfRatings.Should().Be(1);
        enrollment.Review.Should().Be("Excellent course.");
        enrollment.ReviewedAtUTC.Should().NotBeNull();
    }

    [Fact]
    public void ReviewCourse_WhenEnrollmentIsNotCompleted_ThrowsInvalidOperationException()
    {
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid(), EnrollmentStatus.Active);

        var action = () => enrollment.ReviewCourse(4, "Not allowed yet.");

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*before completing*");
    }

    [Fact]
    public void ReviewCourse_WhenCalledTwice_ThrowsInvalidOperationException()
    {
        var enrollment = CreateCompletedEnrollment();
        enrollment.ReviewCourse(4, "First review.");

        var action = () => enrollment.ReviewCourse(5, "Second review.");

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*only submit one review*");
    }

    [Fact]
    public void AddReviewRating_WhenMultipleRatingsAreAdded_UpdatesAverageRating()
    {
        var course = new Course(
            "DDD Foundations",
            "A production-inspired architecture course.",
            Guid.NewGuid(),
            CourseCategory.Programming,
            CourseLevel.Intermediate,
            Duration.Create(2, 30),
            99);

        course.AddReviewRating(4);
        course.AddReviewRating(5);

        course.AverageRating.Value.Should().Be(4.5m);
        course.AverageRating.NumberOfRatings.Should().Be(2);
    }

    private static Enrollment CreateCompletedEnrollment()
    {
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid(), EnrollmentStatus.Active);
        enrollment.CompleteLesson(Guid.NewGuid(), totalLessonsInCourse: 1, requiredAssignmentIds: []);
        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        return enrollment;
    }
}
