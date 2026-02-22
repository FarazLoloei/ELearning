namespace ELearning.Application.Common.Exceptions;

// Exception for when an entity is not found.
public class NotFoundException : DomainApplicationException
{
    public string? EntityName { get; }
    public object? Key { get; }

    public NotFoundException()
        : base("The requested resource was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' ({key}) was not found.")
    {
        EntityName = entityName;
        Key = key;
    }
}
