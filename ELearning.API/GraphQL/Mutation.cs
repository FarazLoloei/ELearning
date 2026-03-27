// <copyright file="Mutation.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.API.GraphQL;

using ELearning.API.GraphQL.InputTypes;
using ELearning.API.GraphQL.Payloads;
using ELearning.Application.Auth.Commands;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Submissions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// GraphQL mutation root type.
/// </summary>
[GraphQLDescription("The mutation root type for the E-Learning API")]
public class Mutation(ILogger<Mutation> logger)
{
    [GraphQLDescription("Create a new course")]
    [Authorize(Roles = "Instructor")]
    public async Task<CoursePayload> CreateCourse(
        [Service] IMediator mediator,
        CreateCourseInput input)
    {
        try
        {
            var command = new CreateCourseCommand(
                Title: input.Title,
                Description: input.Description,
                CategoryId: input.CategoryId,
                LevelId: input.LevelId,
                Price: input.Price,
                DurationHours: input.DurationHours,
                DurationMinutes: input.DurationMinutes);

            var result = await mediator.Send(command);
            return result.IsSuccess ? new CoursePayload() : new CoursePayload(result.Error);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating course: {Title}", input.Title);
            return new CoursePayload(new Error("CREATION_FAILED", "Failed to create course"));
        }
    }

    [GraphQLDescription("Enroll in a course")]
    [Authorize(Roles = "Student")]
    public async Task<EnrollmentPayload> CreateEnrollment(
        [Service] IMediator mediator,
        CreateEnrollmentInput input)
    {
        var command = new CreateEnrollmentCommand
        {
            CourseId = input.CourseId,
        };

        var result = await mediator.Send(command);
        return result.IsSuccess ? new EnrollmentPayload() : new EnrollmentPayload(result.Error);
    }

    [GraphQLDescription("Submit an assignment")]
    [Authorize(Roles = "Student")]
    public async Task<SubmissionPayload> CreateSubmission(
        [Service] IMediator mediator,
        CreateSubmissionInput input)
    {
        var command = new CreateSubmissionCommand
        {
            AssignmentId = input.AssignmentId,
            Content = input.Content,
            FileUrl = input.FileUrl,
        };

        var result = await mediator.Send(command);
        return result.IsSuccess ? new SubmissionPayload() : new SubmissionPayload(result.Error);
    }

    [GraphQLDescription("Grade a submission")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<GradeSubmissionPayload> GradeSubmission(
        [Service] IMediator mediator,
        GradeSubmissionInput input)
    {
        var command = new GradeSubmissionCommand
        {
            SubmissionId = input.SubmissionId,
            Score = input.Score,
            Feedback = input.Feedback,
        };

        var result = await mediator.Send(command);
        return result.IsSuccess ? new GradeSubmissionPayload() : new GradeSubmissionPayload(result.Error);
    }

    [GraphQLDescription("Update an existing course")]
    [Authorize(Roles = "Instructor")]
    public async Task<OperationPayload> UpdateCourse(
        [Service] IMediator mediator,
        UpdateCourseInput input)
    {
        var command = new UpdateCourseCommand(
            input.CourseId,
            input.Title,
            input.Description,
            input.CategoryId,
            input.LevelId,
            input.Price,
            input.DurationHours,
            input.DurationMinutes,
            input.IsFeatured);

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_UPDATE_ERROR", result.Error));
    }

    [GraphQLDescription("Delete a course")]
    [Authorize(Roles = "Instructor,Admin")]
    public async Task<OperationPayload> DeleteCourse(
        [Service] IMediator mediator,
        Guid courseId)
    {
        var result = await mediator.Send(new DeleteCourseCommand(courseId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_DELETE_ERROR", result.Error));
    }

    [GraphQLDescription("Submit a course for admin review")]
    [Authorize(Roles = "Instructor")]
    public async Task<OperationPayload> SubmitCourseForReview(
        [Service] IMediator mediator,
        Guid courseId)
    {
        var result = await mediator.Send(new SubmitCourseForReviewCommand(courseId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_REVIEW_SUBMISSION_ERROR", result.Error));
    }

    [GraphQLDescription("Approve a course for publication")]
    [Authorize(Roles = "Admin")]
    public async Task<OperationPayload> ApproveCoursePublication(
        [Service] IMediator mediator,
        Guid courseId)
    {
        var result = await mediator.Send(new ApproveCoursePublicationCommand(courseId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_APPROVAL_ERROR", result.Error));
    }

    [GraphQLDescription("Reject a course review submission")]
    [Authorize(Roles = "Admin")]
    public async Task<OperationPayload> RejectCoursePublication(
        [Service] IMediator mediator,
        Guid courseId,
        string reason)
    {
        var result = await mediator.Send(new RejectCoursePublicationCommand(courseId, reason));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_REJECTION_ERROR", result.Error));
    }

    [GraphQLDescription("Archive a course")]
    [Authorize(Roles = "Admin")]
    public async Task<OperationPayload> ArchiveCourse(
        [Service] IMediator mediator,
        Guid courseId)
    {
        var result = await mediator.Send(new ArchiveCourseCommand(courseId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("COURSE_ARCHIVE_ERROR", result.Error));
    }

    [GraphQLDescription("Mark a lesson as started for an enrolled student")]
    [Authorize(Roles = "Student")]
    public async Task<OperationPayload> StartLesson(
        [Service] IMediator mediator,
        Guid enrollmentId,
        Guid lessonId)
    {
        var result = await mediator.Send(new StartLessonCommand(enrollmentId, lessonId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("LESSON_START_ERROR", result.Error));
    }

    [GraphQLDescription("Mark a lesson as completed for an enrolled student")]
    [Authorize(Roles = "Student")]
    public async Task<OperationPayload> CompleteLesson(
        [Service] IMediator mediator,
        Guid enrollmentId,
        Guid lessonId)
    {
        var result = await mediator.Send(new CompleteLessonCommand(enrollmentId, lessonId));
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("LESSON_COMPLETION_ERROR", result.Error));
    }

    [GraphQLDescription("Pause, resume, or abandon an enrollment without affecting completion rules")]
    [Authorize(Roles = "Student,Admin")]
    public async Task<OperationPayload> UpdateEnrollmentStatus(
        [Service] IMediator mediator,
        UpdateEnrollmentStatusInput input)
    {
        var command = new UpdateEnrollmentStatusCommand
        {
            EnrollmentId = input.EnrollmentId,
            Status = input.Status,
        };

        var result = await mediator.Send(command);
        return result.IsSuccess
            ? new OperationPayload()
            : new OperationPayload(new Error("ENROLLMENT_STATUS_ERROR", result.Error));
    }

    [GraphQLDescription("Authenticate a user and return token details")]
    public async Task<AuthResult> Login(
        [Service] IMediator mediator,
        LoginInput input,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new AuthenticateUserCommand(input.Email, input.Password), cancellationToken);
    }

    [GraphQLDescription("Register a student account")]
    public async Task<AuthResult> RegisterStudent(
        [Service] IMediator mediator,
        RegisterStudentInput input,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(
            new RegisterStudentCommand(
                input.FirstName,
                input.LastName,
                input.Email,
                input.Password),
            cancellationToken);
    }

    [GraphQLDescription("Register an instructor account")]
    public async Task<AuthResult> RegisterInstructor(
        [Service] IMediator mediator,
        RegisterInstructorInput input,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(
            new RegisterInstructorCommand(
                input.FirstName,
                input.LastName,
                input.Email,
                input.Password,
                input.Bio,
                input.Expertise),
            cancellationToken);
    }
}
