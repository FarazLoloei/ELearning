using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Progress : BaseEntity
{
    /// <summary>
    /// Reference to parent enrollment
    /// </summary>
    public Guid EnrollmentId { get; private set; }

    /// <summary>
    /// Reference to specific lesson
    /// </summary>
    public Guid LessonId { get; private set; }

    /// <summary>
    /// Completion state (NotStarted, InProgress, Completed)
    /// </summary>
    public ProgressStatus Status { get; private set; }

    /// <summary>
    /// When student finished the lesson
    /// </summary>
    public DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Total time student spent on this lesson (in seconds)
    /// </summary>
    public int TimeSpentSeconds { get; private set; }

    private Progress()
    { }

    public Progress(Guid enrollmentId, Guid lessonId, int? timeSpendInSeconds)
    {
        EnrollmentId = enrollmentId;
        LessonId = lessonId;
        Status = ProgressStatus.NotStarted;
        TimeSpentSeconds = timeSpendInSeconds ?? 0;
    }

    public void MarkAsStarted()
    {
        if (Status == ProgressStatus.NotStarted)
        {
            Status = ProgressStatus.InProgress;
            UpdatedAt(DateTime.UtcNow);
        }
    }

    public void MarkAsCompleted()
    {
        Status = ProgressStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        UpdatedAt(DateTime.UtcNow);
    }

    public void AddTimeSpent(int seconds)
    {
        if (seconds <= 0)
            throw new ArgumentException("Time spent must be positive", nameof(seconds));

        TimeSpentSeconds += seconds;
        UpdatedAt(DateTime.UtcNow);
    }
}