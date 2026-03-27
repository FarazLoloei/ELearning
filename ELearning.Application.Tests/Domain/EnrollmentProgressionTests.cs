// <copyright file="EnrollmentProgressionTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Domain;

using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using FluentAssertions;

public sealed class EnrollmentProgressionTests
{
    [Fact]
    public void StartLesson_WhenEnrollmentIsActive_CreatesInProgressLessonProgress()
    {
        var enrollment = CreateEnrollment();
        var lessonId = Guid.NewGuid();

        enrollment.StartLesson(lessonId);

        enrollment.ProgressRecords.Should().ContainSingle(progress => progress.LessonId == lessonId);
        enrollment.ProgressRecords.Single().Status.Should().Be(ProgressStatus.InProgress);
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
    }

    [Fact]
    public void CompleteLesson_WhenLessonIsCompleted_MarksLessonProgressAsCompleted()
    {
        var enrollment = CreateEnrollment();
        var lessonId = Guid.NewGuid();

        enrollment.CompleteLesson(lessonId, totalLessonsInCourse: 2);

        enrollment.ProgressRecords.Should().ContainSingle(progress => progress.LessonId == lessonId);
        enrollment.ProgressRecords.Single().Status.Should().Be(ProgressStatus.Completed);
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
    }

    [Fact]
    public void CompleteLesson_WhenAllLessonsAreCompleted_MarksEnrollmentAsCompleted()
    {
        var enrollment = CreateEnrollment();
        var firstLessonId = Guid.NewGuid();
        var secondLessonId = Guid.NewGuid();

        enrollment.CompleteLesson(firstLessonId, totalLessonsInCourse: 2);
        enrollment.CompleteLesson(secondLessonId, totalLessonsInCourse: 2);

        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        enrollment.CompletedDateUTC.Should().NotBeNull();
    }

    [Fact]
    public void StartLesson_WhenEnrollmentIsPaused_ThrowsInvalidOperationException()
    {
        var enrollment = CreateEnrollment();
        enrollment.Pause();

        var action = () => enrollment.StartLesson(Guid.NewGuid());

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*active enrollments*");
    }

    [Fact]
    public void CompleteLesson_WhenCalledTwiceForSameLesson_DoesNotCreateDuplicateProgressRecords()
    {
        var enrollment = CreateEnrollment();
        var lessonId = Guid.NewGuid();

        enrollment.CompleteLesson(lessonId, totalLessonsInCourse: 2);
        enrollment.CompleteLesson(lessonId, totalLessonsInCourse: 2);

        enrollment.ProgressRecords.Should().ContainSingle(progress => progress.LessonId == lessonId);
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
    }

    private static Enrollment CreateEnrollment() => new(Guid.NewGuid(), Guid.NewGuid());
}
