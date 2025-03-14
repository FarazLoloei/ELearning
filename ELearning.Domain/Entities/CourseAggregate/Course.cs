using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate;

public class Course : BaseEntity, IAggregateRoot<Course>
{
    private readonly List<Module> _modules = new List<Module>();
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    /// <summary>
    /// Name of the course.
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// Detailed explanation of course content and objectives.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Reference to the instructor who created/teaches the course.
    /// </summary>
    public Guid InstructorId { get; private set; }

    /// <summary>
    /// Current state of the course (Draft, Published, Unpublished, Archived).
    /// </summary>
    public CourseStatus Status { get; private set; }

    /// <summary>
    /// Difficulty level of the course (Beginner, Intermediate, Advanced, AllLevels).
    /// </summary>
    public CourseLevel Level { get; private set; }

    /// <summary>
    /// Total estimated time to complete the course.
    /// </summary>
    public Duration Duration { get; private set; }

    /// <summary>
    /// Calculated average rating from all student reviews.
    /// </summary>
    public Rating AverageRating { get; private set; }

    /// <summary>
    /// Subject category of the course (e.g., Programming, Design, Business, etc.).
    /// </summary>
    public CourseCategory Category { get; private set; }

    /// <summary>
    /// The date when the course was made available to students.
    /// </summary>
    public DateTime PublishedDate { get; private set; }

    /// <summary>
    /// Boolean indicating if the course is highlighted on the platform.
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Cost to enroll in the course.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Collection of content modules making up the course.
    /// </summary>
    public IReadOnlyCollection<Module> Modules => _modules.AsReadOnly();

    /// <summary>
    /// Collection of student enrollments in this course.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    // Private constructor for EF Core
    private Course()
    { }

    public Course(
        string title,
        string description,
        Guid instructorId,
        CourseCategory category,
        CourseLevel level,
        Duration duration,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Course title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Course description cannot be empty", nameof(description));

        Title = title;
        Description = description;
        InstructorId = instructorId;
        Category = category;
        Level = level;
        Duration = duration;
        Price = price;
        Status = CourseStatus.Draft;
        AverageRating = Rating.CreateDefault();

        AddDomainEvent(new CourseCreatedEvent(this));
    }

    public void AddModule(Module module)
    {
        _modules.Add(module);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveModule(Module module)
    {
        _modules.Remove(module);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, CourseCategory category, CourseLevel level)
    {
        Title = title;
        Description = description;
        Category = category;
        Level = level;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (_modules.Count == 0)
            throw new InvalidOperationException("Cannot publish a course without any modules");

        Status = CourseStatus.Published;
        PublishedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        Status = CourseStatus.Unpublished;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ToggleFeatured()
    {
        IsFeatured = !IsFeatured;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(Rating newRating)
    {
        AverageRating = newRating;
        UpdatedAt = DateTime.UtcNow;
    }
}