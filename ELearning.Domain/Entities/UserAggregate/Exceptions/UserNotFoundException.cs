using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.UserAggregate.Exceptions;

public sealed class UserNotFoundException : DomainException
{
    public UserNotFoundException(Guid userId)
        : base($"User with ID '{userId}' was not found.")
    { }
}