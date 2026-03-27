// <copyright file="Assignment.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate;

using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel;

public class Assignment : BaseEntity
{
    /// <summary>
    /// Gets name of the assignment.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets instructions and requirements.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets format (Quiz, Essay, Project, Exam).
    /// </summary>
    public AssignmentType Type { get; private set; } = null!;

    /// <summary>
    /// Gets maximum score possible.
    /// </summary>
    public int MaxPoints { get; private set; }

    /// <summary>
    /// Gets deadline for submission.
    /// </summary>
    public DateTime? DueDate { get; private set; }

    /// <summary>
    /// Gets reference to the parent module.
    /// </summary>
    public Guid ModuleId { get; private set; }

    // Private constructor for EF Core
    private Assignment()
    {
    }

    public Assignment(
        string title,
        string description,
        AssignmentType type,
        int maxPoints,
        Guid moduleId,
        DateTime? dueDate = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Assignment title cannot be empty", nameof(title));
        }

        if (maxPoints <= 0)
        {
            throw new ArgumentException("Maximum points must be positive", nameof(maxPoints));
        }

        this.Title = title;
        this.Description = description;
        this.Type = type;
        this.MaxPoints = maxPoints;
        this.ModuleId = moduleId;
        this.DueDate = dueDate;
    }

    public void UpdateDetails(string title, string description, AssignmentType type, int maxPoints)
    {
        if (maxPoints <= 0)
        {
            throw new ArgumentException("Maximum points must be positive", nameof(maxPoints));
        }

        this.Title = title;
        this.Description = description;
        this.Type = type;
        this.MaxPoints = maxPoints;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void SetDueDate(DateTime? dueDate)
    {
        this.DueDate = dueDate;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void EnsureCanAcceptSubmissionAt(DateTime submissionDate)
    {
        if (this.DueDate.HasValue && submissionDate > this.DueDate.Value)
        {
            throw new InvalidOperationException("The submission deadline for this assessment has passed.");
        }
    }

    public void EnsureValidScore(int score)
    {
        if (score < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(score), "Score cannot be negative.");
        }

        if (score > this.MaxPoints)
        {
            throw new ArgumentOutOfRangeException(nameof(score), $"Score cannot exceed maximum points ({this.MaxPoints}).");
        }
    }
}
