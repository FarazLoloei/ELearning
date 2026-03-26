// <copyright file="Submission.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate;

using ELearning.Domain.Entities.EnrollmentAggregate.Events;
using ELearning.SharedKernel;

public class Submission : BaseEntity
{
    /// <summary>
    /// Gets reference to parent enrollment.
    /// </summary>
    public Guid EnrollmentId { get; private set; }

    /// <summary>
    /// Gets reference to the specific assignment.
    /// </summary>
    public Guid AssignmentId { get; private set; }

    /// <summary>
    /// Gets text content of submission (if text-based).
    /// </summary>
    public string Content { get; private set; } = string.Empty;

    /// <summary>
    /// Gets link to uploaded file (if file-based).
    /// </summary>
    public string FileUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether whether submission has been evaluated.
    /// </summary>
    public bool IsGraded { get; private set; }

    /// <summary>
    /// Gets points earned (after grading).
    /// </summary>
    public int? Score { get; private set; }

    /// <summary>
    /// Gets instructor comments on submission.
    /// </summary>
    public string Feedback { get; private set; } = string.Empty;

    /// <summary>
    /// Gets when student submitted the assignment.
    /// </summary>
    public DateTime SubmittedDate { get; private set; }

    /// <summary>
    /// Gets reference to instructor who evaluated work.
    /// </summary>
    public Guid? GradedById { get; private set; }

    /// <summary>
    /// Gets when submission was graded.
    /// </summary>
    public DateTime? GradedDate { get; private set; }

    private Submission()
    {
    }

    public Submission(Guid enrollmentId, Guid assignmentId, string? content = null, string? fileUrl = null, string? feedback = null)
    {
        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new ArgumentException("Submission must have either content or file");
        }

        this.EnrollmentId = enrollmentId;
        this.AssignmentId = assignmentId;
        this.Content = content ?? string.Empty;
        this.FileUrl = fileUrl ?? string.Empty;
        this.IsGraded = false;
        this.Feedback = feedback ?? string.Empty;
        this.SubmittedDate = DateTime.UtcNow;
    }

    public void Grade(int score, string feedback, Guid gradedById)
    {
        if (this.IsGraded)
        {
            throw new InvalidOperationException("Submission is already graded");
        }

        this.Score = score;
        this.Feedback = feedback;
        this.GradedById = gradedById;
        this.GradedDate = DateTime.UtcNow;
        this.IsGraded = true;
        this.UpdatedAt(DateTime.UtcNow);

        this.AddDomainEvent(new SubmissionGradedEvent(this));
    }

    public void UpdateSubmission(string content, string fileUrl)
    {
        if (this.IsGraded)
        {
            throw new InvalidOperationException("Cannot update a graded submission");
        }

        if (string.IsNullOrWhiteSpace(content) && string.IsNullOrWhiteSpace(fileUrl))
        {
            throw new ArgumentException("Submission must have either content or file");
        }

        this.Content = content;
        this.FileUrl = fileUrl;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
