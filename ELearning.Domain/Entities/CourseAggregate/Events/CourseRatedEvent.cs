using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public sealed class CourseRatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public Rating Rating { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseRatedEvent(Student student, Course course, Enrollment enrollment, Rating rating)
    {
        Student = student ?? throw new ArgumentNullException(nameof(student));
        Course = course ?? throw new ArgumentNullException(nameof(course));
        Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        Rating = rating ?? throw new ArgumentNullException(nameof(rating));
        OccurredOnUTC = DateTime.UtcNow;
    }
}