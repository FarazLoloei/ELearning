using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

public sealed class EnrollmentCreatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOnUTC { get; }

    public EnrollmentCreatedEvent(Student student, Course course, Enrollment enrollment)
    {
        Student = student ?? throw new ArgumentNullException(nameof(student));
        Course = course ?? throw new ArgumentNullException(nameof(course));
        Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
        OccurredOnUTC = DateTime.UtcNow;
    }
}