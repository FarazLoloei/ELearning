namespace ELearning.Application.Common.Exceptions;

// Exception for validation errors
public class ValidationException : DomainApplicationException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<(string PropertyName, string ErrorMessage)> failures)
        : this()
    {
        var failureGroups = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());

        Errors = failureGroups;
    }
}