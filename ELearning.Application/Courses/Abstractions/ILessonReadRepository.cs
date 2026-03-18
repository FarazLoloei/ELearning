using ELearning.Application.Courses.ReadModels;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Abstractions;

public interface ILessonReadRepository : IReadRepository<LessonReadModel, Guid>
{
    Task<IReadOnlyList<LessonReadModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default);
}