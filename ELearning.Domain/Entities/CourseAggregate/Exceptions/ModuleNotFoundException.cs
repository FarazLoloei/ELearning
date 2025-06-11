using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

public sealed class ModuleNotFoundException : DomainException
{
    public ModuleNotFoundException(Guid moduleId)
        : base($"Module with ID '{moduleId}' was not found.")
    { }
}