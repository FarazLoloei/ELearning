using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public sealed class CourseCreatedEvent : IDomainEvent
{
    public Course Course { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseCreatedEvent(Course course)
    {
        Course = course ?? throw new ArgumentNullException(nameof(course));
        OccurredOnUTC = DateTime.UtcNow;
    }
}