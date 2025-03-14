using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Enrollment : BaseEntity, IAggregateRoot<Enrollment>
{
    /// <summary>
    /// Reference to enrolled student
    /// </summary>
    public Guid StudentId { get; private set; }

    /// <summary>
    /// Reference to the course being taken
    /// </summary>
    public Guid CourseId { get; private set; }

    /// <summary>
    /// Current state (Active, Paused, Completed, Abandoned)
    /// </summary>
    public EnrollmentStatus Status { get; private set; }

    /// <summary>
    /// When student finished the course (if applicable)
    /// </summary>
    public DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Rating (1-5) given by student after completion
    /// </summary>
    public Rating? CourseRating { get; private set; }

    /// <summary>
    /// Written feedback from student about their experience with the course
    /// </summary>
    public string? Review { get; private set; }

    /// <summary>
    /// Collection tracking individual lesson completion
    /// </summary>
    private readonly List<Progress> _progressRecords = new List<Progress>();

    /// <summary>
    /// Collection of student's assignment submissions
    /// </summary>
    private readonly List<Submission> _submissions = new List<Submission>();

    /// <summary>
    /// Read-only collection tracking individual lesson completion
    /// </summary>
    public IReadOnlyCollection<Progress> ProgressRecords => _progressRecords.AsReadOnly();

    /// <summary>
    /// Read-only collection of student's assignment submissions
    /// </summary>
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    private Enrollment()
    { }

    public Enrollment(Guid studentId, Guid courseId, EnrollmentStatus? enrollmentStatus, string? review)
    {
        StudentId = studentId;
        CourseId = courseId;
        Status = enrollmentStatus ?? EnrollmentStatus.Active;
        Review = review;
    }

    public void MarkAsCompleted()
    {
        Status = EnrollmentStatus.Completed;
        CompletedDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddProgress(Progress progress)
    {
        _progressRecords.Add(progress);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSubmission(Submission submission)
    {
        _submissions.Add(submission);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RateCourse(Rating rating, string review = null)
    {
        if (Status != EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot rate a course before completing it");

        CourseRating = rating;
        Review = review;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(EnrollmentStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}