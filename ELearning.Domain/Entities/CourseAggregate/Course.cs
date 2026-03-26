// <copyright file="Course.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CourseAggregate;

using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.CourseAggregate.Events;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

public class Course : BaseEntity, IAggregateRoot<Course>
{
    private readonly List<Module> modules = new List<Module>();

    private readonly List<Enrollment> enrollments = new List<Enrollment>();

    /// <summary>
    /// Gets name of the course.
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// Gets detailed explanation of course content and objectives.
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Gets reference to the instructor who created/teaches the course.
    /// </summary>
    public Guid InstructorId { get; private set; }

    /// <summary>
    /// Gets current state of the course (Draft, Published, Unpublished, Archived).
    /// </summary>
    public CourseStatus Status { get; private set; } = null!;

    /// <summary>
    /// Gets difficulty level of the course (Beginner, Intermediate, Advanced, AllLevels).
    /// </summary>
    public CourseLevel Level { get; private set; } = null!;

    /// <summary>
    /// Gets total estimated time to complete the course.
    /// </summary>
    public Duration Duration { get; private set; } = null!;

    /// <summary>
    /// Gets calculated average rating from all student reviews.
    /// </summary>
    public Rating AverageRating { get; private set; } = null!;

    /// <summary>
    /// Gets subject category of the course (e.g., Programming, Design, Business, etc.).
    /// </summary>
    public CourseCategory Category { get; private set; } = null!;

    /// <summary>
    /// Gets the date when the course was made available to students.
    /// </summary>
    public DateTime PublishedDate { get; private set; }

    /// <summary>
    /// Gets a value indicating whether boolean indicating if the course is highlighted on the platform.
    /// </summary>
    public bool IsFeatured { get; private set; }

    /// <summary>
    /// Gets cost to enroll in the course.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Gets optimistic concurrency token.
    /// </summary>
    public byte[]? RowVersion { get; private set; }

    /// <summary>
    /// Gets collection of content modules making up the course.
    /// </summary>
    public IReadOnlyCollection<Module> Modules => this.modules.AsReadOnly();

    /// <summary>
    /// Gets collection of student enrollments in this course.
    /// </summary>
    public IReadOnlyCollection<Enrollment> Enrollments => this.enrollments.AsReadOnly();

    // Private constructor for EF Core
    private Course()
    {
    }

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
        {
            throw new ArgumentException("Course title cannot be empty", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Course description cannot be empty", nameof(description));
        }

        this.Title = title;
        this.Description = description;
        this.InstructorId = instructorId;
        this.Category = category;
        this.Level = level;
        this.Duration = duration;
        this.Price = price;
        this.Status = CourseStatus.Draft;
        this.AverageRating = Rating.CreateDefault();

        this.AddDomainEvent(new CourseCreatedEvent(this));
    }

    public void AddModule(Module module)
    {
        this.modules.Add(module);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RemoveModule(Module module)
    {
        this.modules.Remove(module);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateDetails(string title, string description, CourseCategory category, CourseLevel level)
    {
        this.Title = title;
        this.Description = description;
        this.Category = category;
        this.Level = level;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }

        this.Price = price;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Publish()
    {
        if (this.modules.Count == 0)
        {
            throw new InvalidOperationException("Cannot publish a course without any modules");
        }

        this.Status = CourseStatus.Published;
        this.PublishedDate = DateTime.UtcNow;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Unpublish()
    {
        this.Status = CourseStatus.Unpublished;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void ToggleFeatured()
    {
        this.IsFeatured = !this.IsFeatured;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateRating(Rating newRating)
    {
        this.AverageRating = newRating;
        this.UpdatedAt(DateTime.UtcNow);
    }
}