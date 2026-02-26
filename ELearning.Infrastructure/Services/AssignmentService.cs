using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;

namespace ELearning.Infrastructure.Services;

public class AssignmentService(
    IAssignmentRepository assignmentRepository,
    IEnrollmentRepository enrollmentRepository) : IAssignmentService
{
    public async Task<bool> CanSubmitAssignmentAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var module = await assignmentRepository.GetModuleForAssignmentAsync(assignmentId, cancellationToken);
        if (module is null)
            return false;

        var enrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, module.CourseId, cancellationToken);
        if (enrollment is null)
            return false;

        return enrollment.Status == EnrollmentStatus.Active || enrollment.Status == EnrollmentStatus.Completed;
    }

    public async Task<bool> IsAssignmentOverdueAsync(Guid assignmentId, DateTime submissionDate, CancellationToken cancellationToken = default)
    {
        var assignment = await assignmentRepository.GetByIdAsync(assignmentId, cancellationToken);
        if (assignment is null || !assignment.DueDate.HasValue)
            return false;

        return submissionDate > assignment.DueDate.Value;
    }

    public async Task<bool> HasStudentSubmittedAsync(Guid studentId, Guid assignmentId, CancellationToken cancellationToken = default)
    {
        var module = await assignmentRepository.GetModuleForAssignmentAsync(assignmentId, cancellationToken);
        if (module is null)
            return false;

        var enrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, module.CourseId, cancellationToken);
        if (enrollment is null)
            return false;

        return enrollment.Submissions.Any(s => s.AssignmentId == assignmentId);
    }
}
