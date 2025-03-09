using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Enrollment : BaseEntity, IAggregateRoot<Enrollment>
{
    private readonly List<Progress> _progressRecords = new List<Progress>();

    private readonly List<Submission> _submissions = new List<Submission>();

    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public EnrollmentStatus Status { get; private set; }

    public DateTime? CompletedDate { get; private set; }

    public Rating? CourseRating { get; private set; }

    public string Review { get; private set; }

    public IReadOnlyCollection<Progress> ProgressRecords => _progressRecords.AsReadOnly();

    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    private Enrollment()
    { }

    public Enrollment(Guid studentId, Guid courseId, EnrollmentStatus? enrollmentStatus)
    {
        StudentId = studentId;
        CourseId = courseId;
        Status = enrollmentStatus ?? EnrollmentStatus.Active;
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