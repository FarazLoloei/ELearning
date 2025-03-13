using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;

namespace ELearning.Infrastructure.Dapr.Abstraction;

// Interface for Course read service
public interface ICourseReadService : IReadService<CourseDetailDto, Guid>
{
    Task<PaginatedList<CourseListDto>> SearchCoursesAsync(string searchTerm, int? categoryId, int? levelId, bool? isFeatured, int pageNumber, int pageSize);

    Task<List<CourseListDto>> GetFeaturedCoursesAsync(int count);

    Task<List<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId);

    Task<List<CourseListDto>> GetCoursesByCategoryAsync(int categoryId);
}