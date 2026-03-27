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

    public DateTime? ReviewedAtUTC { get; private set; }

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

    public void StartLesson(Guid lessonId)
    {
        this.EnsureCanProgress();

        var progress = this.GetOrCreateProgress(lessonId);
        progress.MarkAsStarted();
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void CompleteLesson(
        Guid lessonId,
        int totalLessonsInCourse,
        IReadOnlyCollection<Guid> requiredAssignmentIds)
    {
        this.EnsureCanProgress();

        var progress = this.GetOrCreateProgress(lessonId);
        progress.MarkAsCompleted();
        this.MarkCourseCompletedIfEligible(totalLessonsInCourse, requiredAssignmentIds);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Pause()
    {
        if (this.Status == EnrollmentStatus.Paused)
        {
            return;
        }

        if (this.Status != EnrollmentStatus.Active)
        {
            throw new InvalidOperationException("Only active enrollments can be paused.");
        }

        this.Status = EnrollmentStatus.Paused;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Resume()
    {
        if (this.Status == EnrollmentStatus.Active)
        {
            return;
        }

        if (this.Status != EnrollmentStatus.Paused)
        {
            throw new InvalidOperationException("Only paused enrollments can be resumed.");
        }

        this.Status = EnrollmentStatus.Active;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void Abandon()
    {
        if (this.Status == EnrollmentStatus.Abandoned)
        {
            return;
        }

        if (this.Status == EnrollmentStatus.Completed)
        {
            throw new InvalidOperationException("Completed enrollments cannot be abandoned.");
        }

        if (this.Status != EnrollmentStatus.Active && this.Status != EnrollmentStatus.Paused)
        {
            throw new InvalidOperationException("Only active or paused enrollments can be abandoned.");
        }

        this.Status = EnrollmentStatus.Abandoned;
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void AddSubmission(Submission submission)
    {
        ArgumentNullException.ThrowIfNull(submission);

        this.submissions.Add(submission);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public Submission SubmitAssignment(
        Guid assignmentId,
        string? content = null,
        string? fileUrl = null,
        int totalLessonsInCourse = 0,
        IReadOnlyCollection<Guid>? requiredAssignmentIds = null)
    {
        this.EnsureCanSubmitAssessments();

        if (this.submissions.Any(s => s.AssignmentId == assignmentId))
        {
            throw new InvalidOperationException("Assignment has already been submitted for this enrollment.");
        }

        var submission = new Submission(this.Id, assignmentId, content, fileUrl);
        this.submissions.Add(submission);
        this.MarkCourseCompletedIfEligible(totalLessonsInCourse, requiredAssignmentIds ?? Array.Empty<Guid>());
        this.UpdatedAt(DateTime.UtcNow);

        return submission;
    }

    public void GradeSubmission(Guid submissionId, int score, int maxPoints, string feedback, Guid gradedById)
    {
        var submission = this.submissions.FirstOrDefault(s => s.Id == submissionId)
            ?? throw new InvalidOperationException("Submission does not belong to this enrollment.");

        submission.Grade(score, maxPoints, feedback, gradedById);
        this.UpdatedAt(DateTime.UtcNow);
    }

    public void ReviewCourse(decimal ratingValue, string? review = null)
    {
        if (this.Status != EnrollmentStatus.Completed)
        {
            throw new InvalidOperationException("Cannot review a course before completing it.");
        }

        if (this.CourseRating is not null)
        {
            throw new InvalidOperationException("A completed enrollment can only submit one review.");
        }

        if (ratingValue < 1 || ratingValue > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(ratingValue), "Rating must be between 1 and 5.");
        }

        this.CourseRating = Rating.Create(ratingValue, 1);
        this.Review = string.IsNullOrWhiteSpace(review) ? null : review.Trim();
        this.ReviewedAtUTC = DateTime.UtcNow;
        this.UpdatedAt(DateTime.UtcNow);
    }

    private Progress GetOrCreateProgress(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson ID is required.", nameof(lessonId));
        }

        var progress = this.progressRecords.FirstOrDefault(record => record.LessonId == lessonId);
        if (progress is not null)
        {
            return progress;
        }

        progress = new Progress(this.Id, lessonId, timeSpendInSeconds: null);
        this.progressRecords.Add(progress);
        return progress;
    }

    private void EnsureCanProgress()
    {
        if (this.Status != EnrollmentStatus.Active)
        {
            throw new InvalidOperationException("Only active enrollments can record lesson progress.");
        }
    }

    private void EnsureCanSubmitAssessments()
    {
        if (this.Status != EnrollmentStatus.Active)
        {
            throw new InvalidOperationException("Only active enrollments can submit assessments.");
        }
    }

    private void MarkCourseCompletedIfEligible(int totalLessonsInCourse, IReadOnlyCollection<Guid> requiredAssignmentIds)
    {
        if (this.Status == EnrollmentStatus.Completed || totalLessonsInCourse <= 0)
        {
            return;
        }

        var completedLessons = this.progressRecords
            .Where(progress => progress.Status == ProgressStatus.Completed)
            .Select(progress => progress.LessonId)
            .Distinct()
            .Count();

        if (completedLessons < totalLessonsInCourse)
        {
            return;
        }

        var submittedAssignmentIds = this.submissions
            .Select(submission => submission.AssignmentId)
            .Distinct()
            .ToHashSet();

        var hasSatisfiedRequiredAssessments = requiredAssignmentIds.Count == 0 ||
            requiredAssignmentIds.All(submittedAssignmentIds.Contains);

        if (!hasSatisfiedRequiredAssessments)
        {
            return;
        }

        this.Status = EnrollmentStatus.Completed;
        this.CompletedDateUTC = DateTime.UtcNow;
    }
}
