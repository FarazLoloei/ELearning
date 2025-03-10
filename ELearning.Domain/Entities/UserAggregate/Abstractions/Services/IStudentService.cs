namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Services;

// Student service interface
public interface IStudentService
    {
        Task<bool> CanAccessCourseAsync(Guid studentId, Guid courseId);
        Task<bool> HasCompletedAssignmentAsync(Guid studentId, Guid assignmentId);
        Task<int> GetTotalCompletedCoursesAsync(Guid studentId);
    }
