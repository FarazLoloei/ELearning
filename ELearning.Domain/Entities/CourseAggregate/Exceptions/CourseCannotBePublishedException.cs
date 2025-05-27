using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

public sealed class CourseCannotBePublishedException : DomainException
{
    public CourseCannotBePublishedException(string reason)
        : base($"Course cannot be published: '{reason}'")
    { }
}