namespace ELearning.Domain.Exceptions;

[Serializable]
public class InvalidDomainOperationException : DomainException
{
    public string? Operation { get; }

    // Default constructor
    public InvalidDomainOperationException()
    { }

    // Constructor with a custom message
    public InvalidDomainOperationException(string message)
        : base(message)
    { }

    // Constructor with message and inner exception
    public InvalidDomainOperationException(string message, Exception innerException)
        : base(message, innerException)
    { }

    // Constructor with message and operation details
    public InvalidDomainOperationException(string message, string operation)
        : base(message)
    {
        Operation = operation;
    }

    // Constructor for serialization
    protected InvalidDomainOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    { }
}