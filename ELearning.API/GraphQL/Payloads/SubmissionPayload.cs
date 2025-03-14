namespace ELearning.API.GraphQL.Payloads;

public class SubmissionPayload : Payload
{
    public Guid? SubmissionId { get; }

    public SubmissionPayload()//(Guid submissionId)
    {
        //SubmissionId = submissionId;
    }

    public SubmissionPayload(string error)
        : base(new[] { error })
    {
    }
}