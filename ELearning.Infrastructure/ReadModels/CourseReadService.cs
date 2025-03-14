using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Courses.Abstractions.ReadModels;
using ELearning.Application.Courses.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.SharedKernel;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class CourseReadService(DaprClient daprClient, ILogger<CourseReadService> logger) : ICourseReadService
{
    private const string StateStoreName = "coursestore";

    public async Task<CourseDetailDto> GetByIdAsync(Guid id)
    {
        try
        {
            var course = await daprClient.GetStateAsync<CourseDetailDto>(
                StateStoreName,
                id.ToString());

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            return course;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
            throw;
        }
    }

    public async Task<PaginatedList<CourseDetailDto>> ListAsync(int pageNumber, int pageSize)
    {
        try
        {
            // In a real implementation, you would use Dapr queries or bulk get operations
            // For simplicity, we're simulating with a direct state get
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<CourseDetailDto>>(
                httpMethod: HttpMethod.Get,
                "courseservice",
                $"api/courses?pageNumber={pageNumber}&pageSize={pageSize}");

            return new PaginatedList<CourseDetailDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing courses");
            throw;
        }
    }

    public async Task<PaginatedList<CourseListDto>> SearchCoursesAsync(
        string searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        int pageNumber,
        int pageSize)
    {
        try
        {
            var queryParams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(searchTerm))
                queryParams.Add("searchTerm", searchTerm);

            if (categoryId.HasValue)
                queryParams.Add("categoryId", categoryId.Value.ToString());

            if (levelId.HasValue)
                queryParams.Add("levelId", levelId.Value.ToString());

            if (isFeatured.HasValue)
                queryParams.Add("isFeatured", isFeatured.Value.ToString());

            queryParams.Add("pageNumber", pageNumber.ToString());
            queryParams.Add("pageSize", pageSize.ToString());

            var queryString = string.Join("&", queryParams.Select(p => $"{p.Key}={Uri.EscapeDataString(p.Value)}"));

            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<CourseListDto>>(
                HttpMethod.Get,
                "courseservice",
                $"api/courses/search?{queryString}");

            return new PaginatedList<CourseListDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching courses");
            throw;
        }
    }

    public async Task<List<CourseListDto>> GetFeaturedCoursesAsync(int count)
    {
        try
        {
            var featuredCourses = await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                "courseservice",
                $"api/courses/featured?count={count}");

            return featuredCourses;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting featured courses");
            throw;
        }
    }

    public async Task<List<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId)
    {
        try
        {
            var courses = await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                "courseservice",
                $"api/instructors/{instructorId}/courses");

            return courses;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting courses for instructor {InstructorId}", instructorId);
            throw;
        }
    }

    public async Task<List<CourseListDto>> GetCoursesByCategoryAsync(int categoryId)
    {
        try
        {
            var courses = await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                "courseservice",
                $"api/courses/category/{categoryId}");

            return courses;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting courses for category {CategoryId}", categoryId);
            throw;
        }
    }
}