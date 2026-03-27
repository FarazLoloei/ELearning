// <copyright file="Module.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate;

using ELearning.SharedKernel;

public class Module : BaseEntity
{
    private readonly List<Lesson> lessons = new List<Lesson>();

    private readonly List<Assignment> assignments = new List<Assignment>();

    /// <summary>
    /// Gets name of the module.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets explanation of module's learning objectives.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets sequence number within the course (for ordering).
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// Gets reference to the parent course.
    /// </summary>
    public Guid CourseId { get; private set; }

    /// <summary>
    /// Gets collection of lessons within this module.
    /// </summary>
    public IReadOnlyCollection<Lesson> Lessons => this.lessons.AsReadOnly();

    /// <summary>
    /// Gets collection of assignments for assessment.
    /// </summary>
    public IReadOnlyCollection<Assignment> Assignments => this.assignments.AsReadOnly();

    private Module()
    {
    }

    public Module(string title, string description, int order, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Module title cannot be empty", nameof(title));
        }

        this.Title = title;
        this.Description = description;
        this.Order = order;
        this.CourseId = courseId;
    }

    public void AddLesson(Lesson lesson)
    {
        this.lessons.Add(lesson);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RemoveLesson(Lesson lesson)
    {
        this.lessons.Remove(lesson);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddAssignment(Assignment assignment)
    {
        this.assignments.Add(assignment);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RemoveAssignment(Assignment assignment)
    {
        this.assignments.Remove(assignment);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateDetails(string title, string description, int order)
    {
        this.Title = title;
        this.Description = description;
        this.Order = order;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
