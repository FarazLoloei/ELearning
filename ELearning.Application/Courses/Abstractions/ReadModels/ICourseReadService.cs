using ELearning.Application.Courses.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Abstractions.ReadModels;

public interface ICourseReadService : IReadRepository<CourseDetailDto, Guid>
{
    Task<PaginatedList<CourseListDto>> SearchCoursesAsync(string searchTerm, int? categoryId, int? levelId, bool? isFeatured, int pageNumber, int pageSize);

    Task<List<CourseListDto>> GetFeaturedCoursesAsync(int count);

    Task<List<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId);

    Task<List<CourseListDto>> GetCoursesByCategoryAsync(int categoryId);
}