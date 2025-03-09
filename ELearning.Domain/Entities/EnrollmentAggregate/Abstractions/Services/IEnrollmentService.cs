namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Services;

public interface IEnrollmentService
{
    Task<bool> CanStudentEnrollAsync(Guid studentId, Guid courseId);

    Task<bool> HasStudentCompletedPrerequisitesAsync(Guid studentId, Guid courseId);

    Task<double> CalculateCourseCompletionPercentageAsync(Guid enrollmentId);

    Task<bool> CheckIfCourseCompletedAsync(Enrollment enrollment);

    Task ProcessEnrollmentCompletionAsync(Enrollment enrollment);
}