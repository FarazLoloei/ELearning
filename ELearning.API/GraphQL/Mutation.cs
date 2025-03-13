using ELearning.Application.Courses.Commands;
using ELearning.Application.Enrollments.Commands;
using ELearning.Application.Submissions.Commands;
using HotChocolate;
using HotChocolate.Types;
using MediatR;

namespace ELearning.API.GraphQL;

// GraphQL Mutation Type
public class Mutation
{
    // Course mutations
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
            return new CoursePayload(result.Value);
        }

        return new CoursePayload(result.Error);
    }

    // Enrollment mutations
    public async Task<EnrollmentPayload> CreateEnrollment(
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
            return new EnrollmentPayload(result.Value);
        }

        return new EnrollmentPayload(result.Error);
    }

    // Submission mutations
    public async Task<SubmissionPayload> CreateSubmission(
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
            return new SubmissionPayload(result.Value);
        }

        return new SubmissionPayload(result.Error);
    }

    public async Task<GradeSubmissionPayload> GradeSubmission(
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
            return new GradeSubmissionPayload(true);
        }

        return new GradeSubmissionPayload(result.Error);
    }
}
