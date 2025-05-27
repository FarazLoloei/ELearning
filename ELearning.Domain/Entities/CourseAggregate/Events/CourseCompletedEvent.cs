using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public sealed class CourseCompletedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOnUTC { get; }

    public CourseCompletedEvent(Student student, Course course, Enrollment enrollment)
    {
        Student = student ?? throw new ArgumentNullException(nameof(student));
        Course = course ?? throw new ArgumentNullException(nameof(course));
        Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        OccurredOnUTC = DateTime.UtcNow;
    }
}