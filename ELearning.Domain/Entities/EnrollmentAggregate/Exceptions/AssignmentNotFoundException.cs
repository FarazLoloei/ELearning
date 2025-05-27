using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

public sealed class AssignmentNotFoundException : DomainException
{
    public AssignmentNotFoundException(Guid assignmentId)
        : base($"Assignment with ID '{assignmentId}' was not found.")
    { }
}