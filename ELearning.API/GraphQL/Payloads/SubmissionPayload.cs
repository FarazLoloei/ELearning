namespace ELearning.API.GraphQL.Payloads;

public class SubmissionPayload : PayloadBase
{
    public Guid? SubmissionId { get; }

    public SubmissionPayload()//(Guid submissionId)
    {
        //SubmissionId = submissionId;
    }

    public SubmissionPayload(string error)
        : base(new Error("SUBMISSION_ERROR", error))
    {
    }
}
