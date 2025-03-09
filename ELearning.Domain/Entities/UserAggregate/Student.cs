using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

namespace ELearning.Domain.Entities.UserAggregate;

public class Student : User
{
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    private Student() : base()
    {
    }

    public Student(string firstName, string lastName, Email email, string passwordHash)
        : base(firstName, lastName, email, passwordHash, UserRole.Student)
    {
    }

    public void EnrollInCourse(Course course)
    {
        var enrollment = new Enrollment(Id, course.Id);
        _enrollments.Add(enrollment);

        AddDomainEvent(new EnrollmentCreatedEvent(this, course, enrollment));
    }

    public void UnenrollFromCourse(Guid courseId)
    {
        var enrollment = _enrollments.FirstOrDefault(e => e.CourseId == courseId);
        if (enrollment != null)
        {
            _enrollments.Remove(enrollment);
        }
    }
}