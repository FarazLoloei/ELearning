using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate;

public class Submission : BaseEntity
{
    public Guid EnrollmentId { get; private set; }

    public Guid AssignmentId { get; private set; }

    public string Content { get; private set; }

    public string FileUrl { get; private set; }

    public bool IsGraded { get; private set; }

    public int? Score { get; private set; }

    public string Feedback { get; private set; }

    public DateTime SubmittedDate { get; private set; }

    public Guid? GradedById { get; private set; }

    public DateTime? GradedDate { get; private set; }

    private Submission()
    { }

    public Submission(Guid enrollmentId, Guid assignmentId, string content = null, string fileUrl = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("Submission must have either content or file");

        EnrollmentId = enrollmentId;
        AssignmentId = assignmentId;
        Content = content;
        FileUrl = fileUrl;
        IsGraded = false;
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