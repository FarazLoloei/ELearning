// <copyright file="Progress.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate;

using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.SharedKernel;

public class Progress : BaseEntity
{
    /// <summary>
    /// Gets reference to parent enrollment.
    /// </summary>
    public Guid EnrollmentId { get; private set; }

    /// <summary>
    /// Gets reference to specific lesson.
    /// </summary>
    public Guid LessonId { get; private set; }

    /// <summary>
    /// Gets completion state (NotStarted, InProgress, Completed).
    /// </summary>
    public ProgressStatus Status { get; private set; } = ProgressStatus.NotStarted;

    /// <summary>
    /// Gets when student finished the lesson.
    /// </summary>
    public DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Gets total time student spent on this lesson (in seconds).
    /// </summary>
    public int TimeSpentSeconds { get; private set; }

    private Progress()
    {
    }

    public Progress(Guid enrollmentId, Guid lessonId, int? timeSpendInSeconds)
    {
        this.EnrollmentId = enrollmentId;
        this.LessonId = lessonId;
        this.Status = ProgressStatus.NotStarted;
        this.TimeSpentSeconds = timeSpendInSeconds ?? 0;
    }

    public void MarkAsStarted()
    {
        if (this.Status == ProgressStatus.NotStarted)
        {
            this.Status = ProgressStatus.InProgress;
            this.UpdatedAt(DateTime.UtcNow);
        }
    }

    public void MarkAsCompleted()
    {
        this.Status = ProgressStatus.Completed;
        this.CompletedDate = DateTime.UtcNow;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddTimeSpent(int seconds)
    {
        if (seconds <= 0)
        {
            throw new ArgumentException("Time spent must be positive", nameof(seconds));
        }

        this.TimeSpentSeconds += seconds;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
