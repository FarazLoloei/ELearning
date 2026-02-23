namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

public interface ISubmissionRepository
{
    Task<Submission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Submission>> GetByAssignmentIdAsync(Guid assignmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Submission>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    Task<Submission?> GetByStudentAndAssignmentIdAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Submission>> GetUngradedSubmissionsAsync(CancellationToken cancellationToken = default);
}
