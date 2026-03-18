using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Abstractions;

public interface IModuleReadRepository : IReadRepository<ModuleReadModel, Guid>
{
    Task<IReadOnlyList<ModuleReadModel>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default);
}