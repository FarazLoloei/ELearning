// <copyright file="Enrollment.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate;

using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

public class Enrollment : BaseEntity, IAggregateRoot<Enrollment>
{
    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public EnrollmentStatus Status { get; private set; } = EnrollmentStatus.Active;

    public DateTime? CompletedDateUTC { get; private set; }

    public Rating? CourseRating { get; private set; }

    public string? Review { get; private set; }

    /// <summary>
    /// Gets optimistic concurrency token.
    /// </summary>
    public byte[]? RowVersion { get; private set; }

    private readonly List<Progress> progressRecords = new();

    private readonly List<Submission> submissions = new();

    public IReadOnlyCollection<Progress> ProgressRecords => this.progressRecords.AsReadOnly();

    public IReadOnlyCollection<Submission> Submissions => this.submissions.AsReadOnly();

    private Enrollment()
    {
    }

    public Enrollment(Guid studentId, Guid courseId, EnrollmentStatus? enrollmentStatus = null, string? review = null)
    {
        this.StudentId = studentId;
        this.CourseId = courseId;
        this.Status = enrollmentStatus ?? EnrollmentStatus.Active;
        this.Review = review;
    }

    public void MarkAsCompleted()
    {
        this.Status = EnrollmentStatus.Completed;
        this.CompletedDateUTC = DateTime.UtcNow;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddProgress(Progress progress)
    {
        this.progressRecords.Add(progress);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        this.submissions.Add(submission);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public Submission SubmitAssignment(Guid assignmentId, string? content = null, string? fileUrl = null)
    {
        if (this.submissions.Any(s => s.AssignmentId == assignmentId))
        {
            throw new InvalidOperationException("Assignment has already been submitted for this enrollment.");
        }

        var submission = new Submission(this.Id, assignmentId, content, fileUrl);
        this.submissions.Add(submission);
        this.UpdatedAt(DateTime.UtcNow);

        return submission;
    }

    public void GradeSubmission(Guid submissionId, int score, string feedback, Guid gradedById)
    {
        var submission = this.submissions.FirstOrDefault(s => s.Id == submissionId)
            ?? throw new InvalidOperationException("Submission does not belong to this enrollment.");

        submission.Grade(score, feedback, gradedById);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void RateCourse(Rating rating, string? review = null)
    {
        if (this.Status != EnrollmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot rate a course before completing it");
        }

        this.CourseRating = rating;
        this.Review = review;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void SetStatus(EnrollmentStatus status)
    {
        this.Status = status;
        this.UpdatedAt(DateTime.UtcNow);
    }
}
