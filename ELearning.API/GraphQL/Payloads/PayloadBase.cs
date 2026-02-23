namespace ELearning.API.GraphQL.Payloads;

[GraphQLDescription("Base payload for mutation responses")]
public abstract class PayloadBase
{
    //protected PayloadBase(IReadOnlyCollection<string> errors = null) => Errors = errors;

    //public IReadOnlyCollection<CustomError> Errors { get; }

    protected readonly List<Error> errors = new();

    [GraphQLDescription("Indicates whether the operation was successful")]
    public bool IsSuccess => !errors.Any();

    [GraphQLDescription("List of errors that occurred during the operation")]
    [UseFiltering]
    public IReadOnlyList<Error> Errors => errors.AsReadOnly();

    protected PayloadBase()
    { }

    protected PayloadBase(Error error) : this(new[] { error })
    {
    }

    protected PayloadBase(IEnumerable<Error> Errors)
    {
        this.errors.AddRange(Errors);
    }

    public void AddError(Error Error) => errors.Add(Error);

    public void AddError(string Code, string Message) => errors.Add(new Error(Code, Message));
}