using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate;

public class Assignment : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public AssignmentType Type { get; private set; }
    public int MaxPoints { get; private set; }
    public DateTime? DueDate { get; private set; }
    public Guid ModuleId { get; private set; }

    private Assignment()
    { }

    public Assignment(
        string title,
        string description,
        AssignmentType type,
        int maxPoints,
        Guid moduleId,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Assignment title cannot be empty", nameof(title));

        if (maxPoints <= 0)
            throw new ArgumentException("Maximum points must be positive", nameof(maxPoints));

        Title = title;
        Description = description;
        Type = type;
        MaxPoints = maxPoints;
        ModuleId = moduleId;
        DueDate = dueDate;
    }

    public void UpdateDetails(string title, string description, AssignmentType type, int maxPoints)
    {
        if (maxPoints <= 0)
            throw new ArgumentException("Maximum points must be positive", nameof(maxPoints));

        Title = title;
        Description = description;
        Type = type;
        MaxPoints = maxPoints;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDueDate(DateTime? dueDate)
    {
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
}