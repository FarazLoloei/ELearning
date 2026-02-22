namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId);

    Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId);

    Task<Submission?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId);

    Task<IReadOnlyList<Submission>> GetUngradedSubmissionsAsync();

    Task AddAsync(Submission submission);

    Task UpdateAsync(Submission submission);
}
