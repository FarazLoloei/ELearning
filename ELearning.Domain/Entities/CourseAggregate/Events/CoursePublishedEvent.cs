using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public sealed class CoursePublishedEvent : IDomainEvent
{
    public Course Course { get; }

    public DateTime OccurredOnUTC { get; }

    public CoursePublishedEvent(Course course)
    {
        Course = course ?? throw new ArgumentNullException(nameof(course));
        OccurredOnUTC = DateTime.UtcNow;
    }
}