namespace ELearning.Application.Common.Exceptions;

public class DomainApplicationException : Exception
{
    public DomainApplicationException()
        : base()
    {
    }

    public DomainApplicationException(string message)
        : base(message)
    {
    }

    public DomainApplicationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
