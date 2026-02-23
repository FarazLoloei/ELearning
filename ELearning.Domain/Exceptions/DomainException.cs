namespace ELearning.Domain.Exceptions;

[Serializable]
public class DomainException : Exception
{
    public string? ErrorCode { get; }
    public string? Details { get; }

    public DomainException()
    { }

    public DomainException(string message)
        : base(message)
    { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    { }

    public DomainException(string message, string errorCode)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    public DomainException(string message, string errorCode, string details)
        : base(message)
    {
        ErrorCode = errorCode;
        Details = details;
    }

}
