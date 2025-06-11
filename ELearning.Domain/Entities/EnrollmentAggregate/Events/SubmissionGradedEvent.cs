using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

public sealed class SubmissionGradedEvent : IDomainEvent
{
    public Submission Submission { get; }

    public DateTime OccurredOnUTC { get; }

    public SubmissionGradedEvent(Submission submission)
    {
        Submission = submission ?? throw new ArgumentNullException(nameof(submission)); ;
        OccurredOnUTC = DateTime.UtcNow;
    }
}