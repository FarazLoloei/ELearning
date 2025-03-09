using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

namespace ELearning.Domain.Entities.UserAggregate;

public class Instructor : User
{
    private readonly List<Course> _courses = new List<Course>();

    public string Bio { get; private set; }
    public string Expertise { get; private set; }
    public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

    private Instructor() : base()
    {
    }

    public Instructor(
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        string bio = null,
        string expertise = null)
        : base(firstName, lastName, email, passwordHash, UserRole.Instructor)
    {
        Bio = bio;
        Expertise = expertise;
    }

    public void UpdateBio(string bio)
    {
        Bio = bio;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExpertise(string expertise)
    {
        Expertise = expertise;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddCourse(Course course)
    {
        _courses.Add(course);
    }

    public void RemoveCourse(Course course)
    {
        _courses.Remove(course);
    }
}