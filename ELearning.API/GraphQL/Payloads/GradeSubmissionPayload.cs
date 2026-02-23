namespace ELearning.API.GraphQL.Payloads;

public class GradeSubmissionPayload : PayloadBase
{
    public bool Success { get; }

    public GradeSubmissionPayload()//(bool success)
    {
        //Success = success;
    }

    public GradeSubmissionPayload(string error)
        : base(new Error("GRADING_ERROR", error))
    {
        Success = false;
    }
}
