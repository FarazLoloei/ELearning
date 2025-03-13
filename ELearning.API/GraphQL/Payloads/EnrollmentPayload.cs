namespace ELearning.API.GraphQL.Payloads;

public class EnrollmentPayload : Payload
{
    public Guid? EnrollmentId { get; }

    public EnrollmentPayload(Guid enrollmentId)
    {
        EnrollmentId = enrollmentId;
    }

    public EnrollmentPayload(string error)
        : base(new[] { error })
    {
    }
}
