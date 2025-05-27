using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.CourseAggregate.Exceptions;

// Lesson-specific exceptions
public sealed class LessonNotFoundException : DomainException
{
    public LessonNotFoundException(Guid lessonId)
        : base($"Lesson with ID '{lessonId}' was not found.")
    { }
}