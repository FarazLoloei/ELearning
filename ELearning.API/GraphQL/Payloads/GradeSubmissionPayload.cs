namespace ELearning.API.GraphQL.Payloads;

public class GradeSubmissionPayload : Payload
{
    public bool Success { get; }

    public GradeSubmissionPayload()//(bool success)
    {
        //Success = success;
    }

    public GradeSubmissionPayload(string error)
        : base(new[] { error })
    {
        Success = false;
    }
}