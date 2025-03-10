namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;

public interface IAssignmentService
{
    Task<bool> CanSubmitAssignmentAsync(Guid studentId, Guid assignmentId);

    Task<bool> IsAssignmentOverdueAsync(Guid assignmentId, DateTime submissionDate);

    Task<bool> HasStudentSubmittedAsync(Guid studentId, Guid assignmentId);
}