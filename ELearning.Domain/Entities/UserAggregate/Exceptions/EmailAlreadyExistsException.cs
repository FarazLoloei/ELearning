using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.UserAggregate.Exceptions;

public class EmailAlreadyExistsException : DomainException
{
    public EmailAlreadyExistsException(string email)
        : base($"User with email {email} already exists.")
    { }
}