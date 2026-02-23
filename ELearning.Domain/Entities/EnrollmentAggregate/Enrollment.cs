using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Enrollment : BaseEntity, IAggregateRoot<Enrollment>
{
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;

    public DateTime? CompletedDateUTC { get; private set; }

    public Rating? CourseRating { get; private set; }

    public string? Review { get; private set; }

    private readonly List<Progress> _progressRecords = new();

    private readonly List<Submission> _submissions = new();

    public IReadOnlyCollection<Progress> ProgressRecords => _progressRecords.AsReadOnly();

    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    private Enrollment()
    { }

    public Enrollment(Guid studentId, Guid courseId, EnrollmentStatus? enrollmentStatus = null, string? review = null)
    {
        StudentId = studentId;
        CourseId = courseId;
        Status = enrollmentStatus ?? EnrollmentStatus.Active;
        Review = review;
    }

    public void MarkAsCompleted()
    {
        Status = EnrollmentStatus.Completed;
        CompletedDateUTC = DateTime.UtcNow;
        UpdatedAt(DateTime.UtcNow);
    }

    public void AddProgress(Progress progress)
    {
        _progressRecords.Add(progress);
        UpdatedAt(DateTime.UtcNow);
    }

    public void AddSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        _submissions.Add(submission);
        UpdatedAt(DateTime.UtcNow);
    }

    public Submission SubmitAssignment(Guid assignmentId, string? content = null, string? fileUrl = null)
    {
        if (_submissions.Any(s => s.AssignmentId == assignmentId))
            throw new InvalidOperationException("Assignment has already been submitted for this enrollment.");

        var submission = new Submission(Id, assignmentId, content, fileUrl);
        _submissions.Add(submission);
        UpdatedAt(DateTime.UtcNow);

        return submission;
    }

    public void GradeSubmission(Guid submissionId, int score, string feedback, Guid gradedById)
    {
        var submission = _submissions.FirstOrDefault(s => s.Id == submissionId)
            ?? throw new InvalidOperationException("Submission does not belong to this enrollment.");

        submission.Grade(score, feedback, gradedById);
        UpdatedAt(DateTime.UtcNow);
    }

    public void RateCourse(Rating rating, string? review = null)
    {
        if (Status != EnrollmentStatus.Completed)
            throw new InvalidOperationException("Cannot rate a course before completing it");

        CourseRating = rating;
        Review = review;
        UpdatedAt(DateTime.UtcNow);
    }

    public void SetStatus(EnrollmentStatus status)
    {
        Status = status;
        UpdatedAt(DateTime.UtcNow);
    }
}
