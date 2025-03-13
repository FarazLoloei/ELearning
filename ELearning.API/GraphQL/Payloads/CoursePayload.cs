namespace ELearning.API.GraphQL.Payloads;

public class CoursePayload : Payload
{
    public Guid? CourseId { get; }

    public CoursePayload(Guid courseId) => CourseId = courseId;

    public CoursePayload(string error)
        : base(new[] { error })
    {
    }
}