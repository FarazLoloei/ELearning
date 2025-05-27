using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

namespace ELearning.Domain.Entities.UserAggregate;

public class Student : User
{
    private readonly Dictionary<Guid, Enrollment> _enrollments = new();

    /// <summary>
    /// Courses this student is enrolled in
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.Values.ToList().AsReadOnly();

    private Student() : base()
    {
    }

    public Student(string firstName, string lastName, Email email, string passwordHash)
        : base(firstName, lastName, email, passwordHash, UserRole.Student)
    {
    }

    public bool EnrollInCourse(Course course)
    {
        if (course == null) throw new ArgumentNullException(nameof(course));
        if (_enrollments.ContainsKey(course.Id)) return false; // Already enrolled

        var enrollment = new Enrollment(Id, course.Id, null, null);
        _enrollments[course.Id] = enrollment;

        AddDomainEvent(new EnrollmentCreatedEvent(this, course, enrollment));
        return true;
    }

    public bool UnenrollFromCourse(Guid courseId) => _enrollments.Remove(courseId);
}