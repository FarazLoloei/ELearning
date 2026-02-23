using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface ICourseRepository : IEntityFrameworkRepository<Course>
{
    Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId, CancellationToken cancellationToken);

    Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category, CancellationToken cancellationToken);

    Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken);

    Task<IReadOnlyList<Course>> SearchCoursesAsync(string? searchTerm, PaginationParameters pagination, CancellationToken cancellationToken);

    Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count, CancellationToken cancellationToken);

    Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level, CancellationToken cancellationToken);

    Task<int> GetCoursesCountAsync(CancellationToken cancellationToken);
}
