namespace ELearning.Application.Common.Exceptions;

// Exception for when an entity is not found
public class NotFoundException : ApplicationException
{
    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity '{name}' ({key}) was not found.")
    {
    }
}