using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate.Enums;
using ELearning.Domain.ValueObjects;

namespace ELearning.Domain.Entities.UserAggregate;

public class Instructor : User
{
    private readonly HashSet<Course> _courses = new();

    /// <summary>
    /// Professional biography
    /// </summary>
    public string Bio { get; private set; } = string.Empty;

    /// <summary>
    /// Areas of specialization
    /// </summary>
    public string Expertise { get; private set; } = string.Empty;

    /// <summary>
    /// Collection of courses created by this instructor
    /// </summary>
    public IReadOnlyCollection<Course> Courses => _courses.ToList().AsReadOnly();

    private Instructor() : base()
    {
    }

    public Instructor(
        string firstName,
        string lastName,
        Email email,
        string passwordHash,
        string bio = "",
        string expertise = "")
        : base(firstName, lastName, email, passwordHash, UserRole.Instructor)
    {
        Bio = bio ?? string.Empty;
        Expertise = expertise ?? string.Empty;
    }

    public void UpdateBio(string bio)
    {
        if (!string.IsNullOrWhiteSpace(bio))
        {
            Bio = bio;
            UpdatedAt(DateTime.UtcNow);
        }
    }

    public void UpdateExpertise(string expertise)
    {
        if (!string.IsNullOrWhiteSpace(expertise))
        {
            Expertise = expertise;
            UpdatedAt(DateTime.UtcNow);
        }
    }

    public bool AddCourse(Course course) => _courses.Add(course); // HashSet prevents duplicates and returns success status

    public bool RemoveCourse(Course course) => _courses.Remove(course);
}
