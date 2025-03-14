using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public interface ICourseRepository : IEntityFrameworkRepository<Course>
{
    Task<IReadOnlyList<Course>> GetByInstructorIdAsync(Guid instructorId);

    Task<IReadOnlyList<Course>> GetByCategoryAsync(CourseCategory category);

    Task<IReadOnlyList<Course>> GetFeaturedCoursesAsync(int count);

    Task<IReadOnlyList<Course>> SearchCoursesAsync(string searchTerm, int pageNumber, int pageSize);

    Task<IReadOnlyList<Course>> GetRecentCoursesAsync(int count);

    Task<IReadOnlyList<Course>> GetByLevelAsync(CourseLevel level);

    Task<int> GetCoursesCountAsync();
}