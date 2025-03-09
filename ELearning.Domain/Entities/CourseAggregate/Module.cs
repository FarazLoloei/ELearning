using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate;

public class Module : BaseEntity
{
    private readonly List<Lesson> _lessons = new List<Lesson>();
    private readonly List<Assignment> _assignments = new List<Assignment>();

    public string Title { get; private set; }
    public string Description { get; private set; }
    public int Order { get; private set; }
    public Guid CourseId { get; private set; }

    public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();
    public IReadOnlyCollection<Assignment> Assignments => _assignments.AsReadOnly();

    private Module()
    { }

    public Module(string title, string description, int order, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Module title cannot be empty", nameof(title));

        Title = title;
        Description = description;
        Order = order;
        CourseId = courseId;
    }

    public void AddLesson(Lesson lesson)
    {
        _lessons.Add(lesson);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLesson(Lesson lesson)
    {
        _lessons.Remove(lesson);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddAssignment(Assignment assignment)
    {
        _assignments.Add(assignment);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveAssignment(Assignment assignment)
    {
        _assignments.Remove(assignment);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string description, int order)
    {
        Title = title;
        Description = description;
        Order = order;
        UpdatedAt = DateTime.UtcNow;
    }
}