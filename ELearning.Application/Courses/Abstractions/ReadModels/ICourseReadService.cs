using ELearning.Application.Courses.Dtos;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Abstractions.ReadModels;

/// <summary>
/// Provides read access to course data.
/// </summary>
public interface ICourseReadService : IReadRepository<CourseDetailDto, Guid>
{
    /// <summary>
    /// Searches courses with optional filters.
    /// </summary>
    /// <param name="searchTerm">Text to search in course titles or descriptions.</param>
    /// <param name="categoryId">Optional category ID filter.</param>
    /// <param name="levelId">Optional level ID filter.</param>
    /// <param name="isFeatured">Optional flag to filter featured courses.</param>
    /// <param name="pageNumber">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    Task<PaginatedList<CourseListDto>> SearchCoursesAsync(
        string searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);

    /// <summary>
    /// Gets a limited list of featured courses.
    /// </summary>
    /// <param name="count">Number of featured courses to retrieve.</param>
    Task<List<CourseListDto>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken);

    /// <summary>
    /// Gets courses associated with a specific instructor.
    /// </summary>
    /// <param name="instructorId">Instructor ID.</param>
    Task<List<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets courses under a specific category.
    /// </summary>
    /// <param name="categoryId">Category ID.</param>
    Task<List<CourseListDto>> GetCoursesByCategoryAsync(int categoryId, CancellationToken cancellationToken);
}