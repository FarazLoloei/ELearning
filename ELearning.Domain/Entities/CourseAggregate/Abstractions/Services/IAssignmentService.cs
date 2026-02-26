namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;

public interface IAssignmentService
{
    Task<bool> CanSubmitAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);

    Task<bool> IsAssignmentOverdueAsync(Guid assignmentId, DateTime submissionDate, CancellationToken cancellationToken = default);

    Task<bool> HasStudentSubmittedAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default);
}
