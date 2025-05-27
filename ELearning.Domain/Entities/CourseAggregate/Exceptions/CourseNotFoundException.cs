using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

public sealed class CourseNotFoundException : DomainException
{
    public CourseNotFoundException(Guid courseId)
        : base($"Course with ID '{courseId}' was not found.")
    { }
}