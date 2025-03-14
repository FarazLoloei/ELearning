using ELearning.API.GraphQL.InputTypes;
using ELearning.API.GraphQL.Payloads;
using ELearning.Application.Courses.Commands;
using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Submissions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace ELearning.API.GraphQL;

/// <summary>
/// GraphQL mutation root type
/// </summary>
[GraphQLDescription("The mutation root type for the E-Learning API")]
public class Mutation
{
    /// <summary>
    /// Create a new course
    /// </summary>
    [GraphQLDescription("Create a new course")]
    [Authorize(Roles = "Instructor")]
    public async Task<CoursePayload> CreateCourse(
        [Service] IMediator mediator,
        CreateCourseInput input)
    {
        var command = new CreateCourseCommand
        {
            Title = input.Title,
            Description = input.Description,
            CategoryId = input.CategoryId,
            LevelId = input.LevelId,
            Price = input.Price,
            DurationHours = input.DurationHours,
            DurationMinutes = input.DurationMinutes
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new CoursePayload();
        }

        return new CoursePayload(result.Error);
    }

    /// <summary>
    /// Enroll in a course
    /// </summary>
    [GraphQLDescription("Enroll in a course")]
    [Authorize(Roles = "Student")]
    public async Task<EnrollmentPayload?> CreateEnrollment(
        [Service] IMediator mediator,
        CreateEnrollmentInput input)
    {
        var command = new CreateEnrollmentCommand
        {
            CourseId = input.CourseId
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new EnrollmentPayload();
        }

        return new EnrollmentPayload(result.Error);
    }

    /// <summary>
    /// Submit an assignment
    /// </summary>
    [GraphQLDescription("Submit an assignment")]
    [Authorize(Roles = "Student")]
    public async Task<SubmissionPayload?> CreateSubmission(
        [Service] IMediator mediator,
        CreateSubmissionInput input)
    {
        var command = new CreateSubmissionCommand
        {
            AssignmentId = input.AssignmentId,
            Content = input.Content,
            FileUrl = input.FileUrl
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new SubmissionPayload();
        }

        return new SubmissionPayload(result.Error);
    }

    /// <summary>
    /// Grade a submission
    /// </summary>
    [GraphQLDescription("Grade a submission")]
    [Authorize(Roles = "Instructor")]
    public async Task<GradeSubmissionPayload?> GradeSubmission(
        [Service] IMediator mediator,
        GradeSubmissionInput input)
    {
        var command = new GradeSubmissionCommand
        {
            SubmissionId = input.SubmissionId,
            Score = input.Score,
            Feedback = input.Feedback
        };

        var result = await mediator.Send(command);

        if (result.IsSuccess)
        {
            return new GradeSubmissionPayload();
        }

        return new GradeSubmissionPayload(result.Error);
    }
}