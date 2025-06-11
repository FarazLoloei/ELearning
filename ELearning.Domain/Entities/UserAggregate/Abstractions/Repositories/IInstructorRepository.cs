using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

/// <summary>
/// Repository interface for Instructor entity with custom read operations.
/// </summary>
public interface IInstructorRepository : IEntityFrameworkRepository<Instructor>
{
    /// <summary>
    /// Retrieves the top instructors based on rating, enrollment, or other criteria.
    /// </summary>
    Task<IReadOnlyList<Instructor>> GetTopInstructorsAsync(int count, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total number of students taught by a specific instructor.
    /// </summary>
    Task<int> GetTotalStudentCountAsync(Guid instructorId, CancellationToken cancellationToken);

    /// <summary>
    /// Calculates the average rating for a specific instructor.
    /// </summary>
    Task<decimal> GetAverageRatingAsync(Guid instructorId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves an instructor along with their courses.
    /// </summary>
    Task<Instructor?> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken);
}