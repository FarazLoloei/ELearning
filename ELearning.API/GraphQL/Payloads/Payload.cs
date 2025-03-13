namespace ELearning.API.GraphQL.Payloads;

public abstract class Payload
{
    protected Payload(IReadOnlyCollection<string> errors = null) => Errors = errors;

    public IReadOnlyCollection<string> Errors { get; }
}