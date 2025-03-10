using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

public class EnrollmentCreatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOn { get; }

    public EnrollmentCreatedEvent(Student student, Course course, Enrollment enrollment)
    {
        Student = student;
        Course = course;
        Enrollment = enrollment;
        OccurredOn = DateTime.UtcNow;
    }
}