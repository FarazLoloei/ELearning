namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Services;

public interface ISubmissionService
{
    Task<bool> CanGradeSubmissionAsync(Guid instructorId, Guid submissionId);

    Task NotifyStudentOfGradedSubmissionAsync(Submission submission);
}