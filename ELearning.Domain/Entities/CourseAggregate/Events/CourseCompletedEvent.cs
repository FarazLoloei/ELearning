using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public class CourseCompletedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public DateTime OccurredOn { get; }

    public CourseCompletedEvent(Student student, Course course, Enrollment enrollment)
    {
        Student = student;
        Course = course;
        Enrollment = enrollment;
        OccurredOn = DateTime.UtcNow;
    }
}