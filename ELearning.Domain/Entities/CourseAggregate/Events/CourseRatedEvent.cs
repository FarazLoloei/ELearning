using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate.Events;

public class CourseRatedEvent : IDomainEvent
{
    public Student Student { get; }

    public Course Course { get; }

    public Enrollment Enrollment { get; }

    public Rating Rating { get; }

    public DateTime OccurredOn { get; }

    public CourseRatedEvent(Student student, Course course, Enrollment enrollment, Rating rating)
    {
        Student = student;
        Course = course;
        Enrollment = enrollment;
        Rating = rating;
        OccurredOn = DateTime.UtcNow;
    }
}