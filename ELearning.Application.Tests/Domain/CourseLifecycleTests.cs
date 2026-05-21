// <copyright file="CourseLifecycleTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Domain;

using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.CourseAggregate.Exceptions;
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

    [Fact]
    public void AddLessonToModule_WhenCourseIsEditable_AddsLessonToModule()
    {
        var course = CreateCourse();
        var module = new Module("Module 1", "Introduction module.", 1, course.Id);
        course.AddModule(module);
        var lesson = new Lesson(
            "Lesson 1",
            "Read this introduction.",
            LessonType.Text,
            1,
            module.Id);

        course.AddLessonToModule(module.Id, lesson);

        module.Lessons.Should().ContainSingle().Which.Should().Be(lesson);
    }

    [Fact]
    public void AddAssignmentToModule_WhenCourseIsEditable_AddsAssignmentToModule()
    {
        var course = CreateCourse();
        var module = new Module("Module 1", "Introduction module.", 1, course.Id);
        course.AddModule(module);
        var assignment = new Assignment(
            "Assignment 1",
            "Complete the introduction check.",
            AssignmentType.Quiz,
            10,
            module.Id);

        course.AddAssignmentToModule(module.Id, assignment);

        module.Assignments.Should().ContainSingle().Which.Should().Be(assignment);
    }

    [Fact]
    public void AddLessonToModule_WhenModuleDoesNotExist_ThrowsModuleNotFoundException()
    {
        var course = CreateCourse();
        var lesson = new Lesson(
            "Lesson 1",
            "Read this introduction.",
            LessonType.Text,
            1,
            Guid.NewGuid());

        var action = () => course.AddLessonToModule(Guid.NewGuid(), lesson);

        action.Should().Throw<ModuleNotFoundException>();
    }

    [Fact]
    public void AddLessonToModule_WhenCourseIsPublished_ThrowsInvalidOperationException()
    {
        var course = CreateReadyForReviewCourse();
        var module = course.Modules.Single();
        course.ApprovePublication();
        var lesson = new Lesson(
            "Lesson 1",
            "Read this introduction.",
            LessonType.Text,
            1,
            module.Id);

        var action = () => course.AddLessonToModule(module.Id, lesson);

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
