using ELearning.Domain.Exceptions;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Exceptions;

public sealed class SubmissionAlreadyGradedException : DomainException
{
    public SubmissionAlreadyGradedException(Guid submissionId)
        : base($"Submission with ID '{submissionId}' has already been graded.")
    { }
}