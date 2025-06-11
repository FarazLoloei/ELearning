using ELearning.Application.Students.Dtos;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Abstractions.ReadModels;

/// <summary>
/// Provides read-only access to student-related data.
/// </summary>
public interface IStudentReadService : IReadRepository<StudentDto, Guid>
{
    /// <summary>
    /// Retrieves a student by their unique identifier.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<StudentDto> GetStudentByIdAsync(Guid studentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the progress report of a student by their unique identifier.
    /// </summary>
    /// <param name="studentId">The unique identifier of the student.</param>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId, CancellationToken cancellationToken = default);
}