using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public class CourseCreatedEvent : IDomainEvent
{
    public Course Course { get; }

    public DateTime OccurredOn { get; }

    public CourseCreatedEvent(Course course)
    {
        Course = course;
        OccurredOn = DateTime.UtcNow;
    }
}