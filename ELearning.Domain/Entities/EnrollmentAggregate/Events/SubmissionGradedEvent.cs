using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Events;

public class SubmissionGradedEvent : IDomainEvent
{
    public Submission Submission { get; }

    public DateTime OccurredOn { get; }

    public SubmissionGradedEvent(Submission submission)
    {
        Submission = submission;
        OccurredOn = DateTime.UtcNow;
    }
}