// <copyright file="AssessmentWorkflowTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Domain;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using FluentAssertions;

public sealed class AssessmentWorkflowTests
{
    [Fact]
    public void CompleteLesson_WhenRequiredAssessmentsAreOutstanding_DoesNotCompleteEnrollment()
    {
        var enrollment = CreateEnrollment();
        var requiredAssignmentId = Guid.NewGuid();

        enrollment.CompleteLesson(
            Guid.NewGuid(),
            totalLessonsInCourse: 1,
            requiredAssignmentIds: [requiredAssignmentId]);

        enrollment.Status.Should().Be(EnrollmentStatus.Active);
        enrollment.CompletedDateUTC.Should().BeNull();
    }

    [Fact]
    public void SubmitAssignment_WhenLessonsAreAlreadyComplete_AndRequiredAssessmentIsSatisfied_CompletesEnrollment()
    {
        var enrollment = CreateEnrollment();
        var requiredAssignmentId = Guid.NewGuid();

        enrollment.CompleteLesson(
            Guid.NewGuid(),
            totalLessonsInCourse: 1,
            requiredAssignmentIds: [requiredAssignmentId]);

        enrollment.SubmitAssignment(
            requiredAssignmentId,
            content: "Assessment attempt",
            totalLessonsInCourse: 1,
            requiredAssignmentIds: [requiredAssignmentId]);

        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        enrollment.CompletedDateUTC.Should().NotBeNull();
    }

    [Fact]
    public void SubmitAssignment_WhenEnrollmentIsPaused_ThrowsInvalidOperationException()
    {
        var enrollment = CreateEnrollment();
        enrollment.Pause();

        var action = () => enrollment.SubmitAssignment(Guid.NewGuid(), content: "Answer");

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*active enrollments*");
    }

    [Fact]
    public void EnsureCanAcceptSubmissionAt_WhenAssignmentIsOverdue_ThrowsInvalidOperationException()
    {
        var assignment = new Assignment(
            "Final assessment",
            "Complete the final assessment.",
            AssignmentType.Exam,
            maxPoints: 100,
            moduleId: Guid.NewGuid(),
            dueDate: DateTime.UtcNow.AddDays(-1));

        var action = () => assignment.EnsureCanAcceptSubmissionAt(DateTime.UtcNow);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*deadline*");
    }

    [Fact]
    public void Grade_WhenScoreExceedsMaximumPoints_ThrowsArgumentOutOfRangeException()
    {
        var submission = new Submission(Guid.NewGuid(), Guid.NewGuid(), content: "Answer");

        var action = () => submission.Grade(101, 100, "Too high", Guid.NewGuid());

        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*maximum points*");
    }

    private static Enrollment CreateEnrollment() => new(Guid.NewGuid(), Guid.NewGuid());
}
