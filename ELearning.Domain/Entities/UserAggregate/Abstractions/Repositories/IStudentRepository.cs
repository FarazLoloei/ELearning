using ELearning.Domain.Entities.CourseAggregate;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

/// <summary>
/// Repository interface for accessing Student-related data operations.
/// </summary>
public interface IStudentRepository : IEntityFrameworkRepository<Student>
{
    /// <summary>
    /// Retrieves students enrolled in a specific course.
    /// </summary>
    Task<IReadOnlyList<Student>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves courses in which a specific student is enrolled.
    /// </summary>
    Task<IReadOnlyList<Course>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total number of students enrolled in a specific course.
    /// </summary>
    Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken);
}