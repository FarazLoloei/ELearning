namespace ELearning.Domain.Exceptions;

public class InvalidDomainOperationException : DomainException
{
    public InvalidDomainOperationException()
    { }

    public InvalidDomainOperationException(string message)
        : base(message)
    { }

    public InvalidDomainOperationException(string message, Exception innerException)
        : base(message, innerException)
    { }
}