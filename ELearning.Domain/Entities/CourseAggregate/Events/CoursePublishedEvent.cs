using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public class CoursePublishedEvent : IDomainEvent
{
    public Course Course { get; }
    public DateTime OccurredOn { get; }

    public CoursePublishedEvent(Course course)
    {
        Course = course;
        OccurredOn = DateTime.UtcNow;
    }
}