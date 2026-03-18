using ELearning.Application.Students.ReadModels;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Students.Abstractions;

public interface IStudentReadRepository : IReadRepository<StudentReadModel, Guid>
{
    // Task<StudentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StudentReadModel>> GetStudentsByCourseIdAsync(Guid courseId, CancellationToken cancellationToken);

    Task<IReadOnlyList<StudentCourseReadModel>> GetCoursesByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);

    Task<int> GetEnrolledStudentCountAsync(Guid courseId, CancellationToken cancellationToken);
}