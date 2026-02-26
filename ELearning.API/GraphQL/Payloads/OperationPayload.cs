namespace ELearning.API.GraphQL.Payloads;

public sealed class OperationPayload : PayloadBase
{
    public OperationPayload()
    {
    }

    public OperationPayload(string error)
        : base(new Error("OPERATION_ERROR", error))
    {
    }

    public OperationPayload(Error error)
        : base(error)
    {
    }
}
