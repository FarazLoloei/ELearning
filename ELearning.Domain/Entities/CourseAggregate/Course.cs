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
    /// Gets current lifecycle state of the course.
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
    public DateTime? PublishedDate { get; private set; }

    /// <summary>
    /// Gets the latest moderation feedback when a review submission is rejected.
    /// </summary>
    public string? RejectionReason { get; private set; }

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

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
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
        this.EnsureEditableByInstructor();
        this.modules.Add(module);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RemoveModule(Module module)
    {
        this.EnsureEditableByInstructor();
        this.modules.Remove(module);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdateDetails(
        string title,
        string description,
        CourseCategory category,
        CourseLevel level,
        Duration duration)
    {
        this.EnsureEditableByInstructor();

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
        this.Category = category;
        this.Level = level;
        this.Duration = duration;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void UpdatePrice(decimal price)
    {
        this.EnsureEditableByInstructor();

        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative", nameof(price));
        }

        this.Price = price;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void SubmitForReview()
    {
        if (this.Status != CourseStatus.Draft && this.Status != CourseStatus.Rejected)
        {
            throw new InvalidOperationException("Only draft or rejected courses can be submitted for review.");
        }

        if (!this.MeetsMinimumStructureRequirements())
        {
            throw new InvalidOperationException("A course must include at least one module before it can be submitted for review.");
        }

        this.Status = CourseStatus.ReadyForReview;
        this.RejectionReason = null;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void ApprovePublication()
    {
        if (this.Status != CourseStatus.ReadyForReview)
        {
            throw new InvalidOperationException("Only courses that are ready for review can be approved for publication.");
        }

        this.Status = CourseStatus.Published;
        this.PublishedDate = DateTime.UtcNow;
        this.RejectionReason = null;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RejectPublication(string reason)
    {
        if (this.Status != CourseStatus.ReadyForReview)
        {
            throw new InvalidOperationException("Only courses that are ready for review can be rejected.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("A rejection reason is required.", nameof(reason));
        }

        this.Status = CourseStatus.Rejected;
        this.RejectionReason = reason.Trim();
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Archive()
    {
        if (this.Status == CourseStatus.Archived)
        {
            throw new InvalidOperationException("The course is already archived.");
        }

        this.Status = CourseStatus.Archived;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void ToggleFeatured()
    {
        this.EnsureEditableByInstructor();
        this.IsFeatured = !this.IsFeatured;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddReviewRating(decimal ratingValue)
    {
        this.AverageRating = this.AverageRating.AddRating(ratingValue);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public bool IsOwnedBy(Guid instructorId) => this.InstructorId == instructorId;

    public bool IsEditableByInstructor() =>
        this.Status == CourseStatus.Draft || this.Status == CourseStatus.Rejected;

    public bool IsPubliclyVisible() => this.Status == CourseStatus.Published;

    public bool IsAvailableForLearning() => this.Status == CourseStatus.Published;

    public bool CanAcceptNewEnrollments() => this.Status == CourseStatus.Published;

    public bool ContainsLesson(Guid lessonId) =>
        this.modules.SelectMany(module => module.Lessons).Any(lesson => lesson.Id == lessonId);

    public bool ContainsAssignment(Guid assignmentId) =>
        this.modules.SelectMany(module => module.Assignments).Any(assignment => assignment.Id == assignmentId);

    public int GetTotalLessonCount() => this.modules.Sum(module => module.Lessons.Count);

    public IReadOnlyCollection<Guid> GetRequiredAssessmentIds() =>
        this.modules
            .SelectMany(module => module.Assignments)
            .Select(assignment => assignment.Id)
            .ToArray();

    public void EnsureCanAcceptNewEnrollments()
    {
        if (!this.CanAcceptNewEnrollments())
        {
            throw new InvalidOperationException("Students can enroll only in published courses.");
        }
    }

    public void EnsureAvailableForLearning()
    {
        if (!this.IsAvailableForLearning())
        {
            throw new InvalidOperationException("Lesson progression is available only for published courses.");
        }
    }

    public void EnsureCanBeDeleted()
    {
        if (!this.IsEditableByInstructor())
        {
            throw new InvalidOperationException("Only draft or rejected courses can be deleted.");
        }

        if (this.enrollments.Count > 0)
        {
            throw new InvalidOperationException("Cannot delete a course with enrollments. Archive it instead.");
        }
    }

    private bool MeetsMinimumStructureRequirements() => this.modules.Count > 0;

    private void EnsureEditableByInstructor()
    {
        if (!this.IsEditableByInstructor())
        {
            throw new InvalidOperationException("Only draft or rejected courses can be edited by the instructor.");
        }
    }
}
