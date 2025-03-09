using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate;

public class Course : BaseEntity, IAggregateRoot<Course>
{
    private readonly List<Module> _modules = new List<Module>();
    private readonly List<Enrollment> _enrollments = new List<Enrollment>();

    public string Title { get; private set; }
    public string Description { get; private set; }
    public Guid InstructorId { get; private set; }
    public CourseStatus Status { get; private set; }
    public CourseLevel Level { get; private set; }
    public Duration Duration { get; private set; }
    public Rating AverageRating { get; private set; }
    public CourseCategory Category { get; private set; }
    public DateTime PublishedDate { get; private set; }
    public bool IsFeatured { get; private set; }
    public decimal Price { get; private set; }

    public IReadOnlyCollection<Module> Modules => _modules.AsReadOnly();
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