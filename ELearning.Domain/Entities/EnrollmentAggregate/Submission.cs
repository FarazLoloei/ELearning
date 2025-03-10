using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Submission : BaseEntity
{
    /// <summary>
    /// Reference to parent enrollment
    /// </summary>
    public Guid EnrollmentId { get; private set; }

    /// <summary>
    /// Reference to the specific assignment
    /// </summary>
    public Guid AssignmentId { get; private set; }

    /// <summary>
    /// Text content of submission (if text-based)
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// Link to uploaded file (if file-based)
    /// </summary>
    public string FileUrl { get; private set; }

    /// <summary>
    /// Whether submission has been evaluated
    /// </summary>
    public bool IsGraded { get; private set; }

    /// <summary>
    /// Points earned (after grading)
    /// </summary>
    public int? Score { get; private set; }

    /// <summary>
    /// Instructor comments on submission
    /// </summary>
    public string Feedback { get; private set; }

    /// <summary>
    /// When student submitted the assignment
    /// </summary>
    public DateTime SubmittedDate { get; private set; }

    /// <summary>
    /// Reference to instructor who evaluated work
    /// </summary>
    public Guid? GradedById { get; private set; }

    /// <summary>
    /// When submission was graded
    /// </summary>
    public DateTime? GradedDate { get; private set; }

    private Submission()
    { }

    public Submission(Guid enrollmentId, Guid assignmentId, string? content = null, string? fileUrl = null, string? feedback = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Submission must have either content or file");

        EnrollmentId = enrollmentId;
        AssignmentId = assignmentId;
        Content = content ?? string.Empty;
        FileUrl = fileUrl ?? string.Empty;
        IsGraded = false;
        Feedback = feedback ?? string.Empty;
        SubmittedDate = DateTime.UtcNow;
    }

    public void Grade(int score, string feedback, Guid gradedById)
    {
        if (IsGraded)
            throw new InvalidOperationException("Submission is already graded");

        Score = score;
        Feedback = feedback;
        GradedById = gradedById;
        GradedDate = DateTime.UtcNow;
        IsGraded = true;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SubmissionGradedEvent(this));
    }

    public void UpdateSubmission(string content, string fileUrl)
    {
        if (IsGraded)
            throw new InvalidOperationException("Cannot update a graded submission");

        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Submission must have either content or file");

        Content = content;
        FileUrl = fileUrl;
        UpdatedAt = DateTime.UtcNow;
    }
}