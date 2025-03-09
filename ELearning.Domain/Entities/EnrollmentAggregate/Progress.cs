using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Progress : BaseEntity
{
    public Guid EnrollmentId { get; private set; }

    public Guid LessonId { get; private set; }

    public ProgressStatus Status { get; private set; }

    public DateTime? CompletedDate { get; private set; }

    public int TimeSpentSeconds { get; private set; }

    private Progress()
    { }

    public Progress(Guid enrollmentId, Guid lessonId)
    {
        EnrollmentId = enrollmentId;
        LessonId = lessonId;
        Status = ProgressStatus.NotStarted;
        TimeSpentSeconds = 0;
    }

    public void MarkAsStarted()
    {
        if (Status == ProgressStatus.NotStarted)
        {
            Status = ProgressStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void MarkAsCompleted()
    {
        Status = ProgressStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTimeSpent(int seconds)
    {
        if (seconds <= 0)
            throw new ArgumentException("Time spent must be positive", nameof(seconds));

        TimeSpentSeconds += seconds;
        UpdatedAt = DateTime.UtcNow;
    }
}