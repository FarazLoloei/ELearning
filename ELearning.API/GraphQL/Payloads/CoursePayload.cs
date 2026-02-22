using ELearning.Application.Courses.Dtos;

namespace ELearning.API.GraphQL.Payloads;

[GraphQLDescription("Payload for course operations")]
public class CoursePayload : PayloadBase
{
    [GraphQLDescription("The course affected by the operation")]
    public CourseDto? Course { get; }

    public CoursePayload()
    {
    }

    public CoursePayload(CourseDto course) => Course = course;

    public CoursePayload(string error) : base(new Error("COURSE_ERROR", error))
    {
    }

    public CoursePayload(Error error) : base(error)
    {
    }

    public CoursePayload(IEnumerable<Error> errors) : base(errors)
    {
    }
}