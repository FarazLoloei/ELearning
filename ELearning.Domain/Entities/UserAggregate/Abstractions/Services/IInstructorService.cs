namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Services;

// Instructor service interface
public interface IInstructorService
{
    Task<bool> CanAccessCourseAsync(Guid instructorId, Guid courseId);

    Task<bool> CanManageAssignmentAsync(Guid instructorId, Guid assignmentId);

    Task<int> GetTotalStudentsAsync(Guid instructorId);

    Task<decimal> GetAverageRatingAsync(Guid instructorId);
}