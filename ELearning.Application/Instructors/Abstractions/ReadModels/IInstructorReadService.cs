using ELearning.Application.Instructors.Dtos;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Instructors.Abstractions.ReadModels;

/// <summary>
/// Provides read-only access to instructor-related data.
/// </summary>
public interface IInstructorReadService : IReadRepository<InstructorDto, Guid>
{
    /// <summary>
    /// Retrieves an instructor by their unique identifier.
    /// </summary>
    /// <param name="instructorId">The unique identifier of the instructor.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<InstructorDto> GetInstructorByIdAsync(Guid instructorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an instructor along with the list of their courses.
    /// </summary>
    /// <param name="instructorId">The unique identifier of the instructor.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken = default);
}