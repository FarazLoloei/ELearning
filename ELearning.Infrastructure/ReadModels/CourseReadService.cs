using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Courses.Abstractions.ReadModels;
using ELearning.Application.Courses.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;
using System.Web;

namespace ELearning.Infrastructure.ReadModels;

public class CourseReadService(DaprClient daprClient, ILogger<CourseReadService> logger) : ICourseReadService
{
    private const string StateStoreName = "coursestore";
    private const string CourseServiceAppId = "courseservice";

    public async Task<CourseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var course = await daprClient.GetStateAsync<CourseDto>(
                StateStoreName,
                id.ToString(),
                cancellationToken: cancellationToken);

            return course == null ? throw new NotFoundException(nameof(Course), id) : course;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving course with ID {CourseId}", id);
            throw;
        }
    }

    public async Task<PaginatedList<CourseDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["pageNumber"] = pagination.PageNumber.ToString();
        query["pageSize"] = pagination.PageSize.ToString();

        return await InvokePaginatedService<CourseDto>(
            $"api/courses?{query}",
            "listing courses",
            pagination,
            cancellationToken);
    }

    public async Task<PaginatedList<CourseListDto>> SearchCoursesAsync(
        string searchTerm,
        int? categoryId,
        int? levelId,
        bool? isFeatured,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["pageNumber"] = pageNumber.ToString();
        query["pageSize"] = pageSize.ToString();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query["searchTerm"] = searchTerm;
        if (categoryId.HasValue)
            query["categoryId"] = categoryId.Value.ToString();
        if (levelId.HasValue)
            query["levelId"] = levelId.Value.ToString();
        if (isFeatured.HasValue)
            query["isFeatured"] = isFeatured.Value.ToString();

        return await InvokePaginatedService<CourseListDto>(
            $"api/courses/search?{query}",
            "searching courses",
            new PaginationParameters(pageNumber, pageSize),
            cancellationToken);
    }

    public async Task<List<CourseListDto>> GetFeaturedCoursesAsync(int count, CancellationToken cancellationToken)
    {
        try
        {
            return await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                CourseServiceAppId,
                $"api/courses/featured?count={count}",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting featured courses");
            throw;
        }
    }

    public async Task<List<CourseListDto>> GetCoursesByInstructorAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        try
        {
            return await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                CourseServiceAppId,
                $"api/instructors/{instructorId}/courses",
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting courses for instructor {InstructorId}", instructorId);
            throw;
        }
    }

    public async Task<List<CourseListDto>> GetCoursesByCategoryAsync(int categoryId, CancellationToken cancellationToken)
    {
        try
        {
            return await daprClient.InvokeMethodAsync<List<CourseListDto>>(
                HttpMethod.Get,
                CourseServiceAppId,
                $"api/courses/category/{categoryId}",
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting courses for category {CategoryId}", categoryId);
            throw;
        }
    }

    private async Task<PaginatedList<T>> InvokePaginatedService<T>(
        string endpoint,
        string operationName,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<T>>(
                HttpMethod.Get,
                CourseServiceAppId,
                endpoint,
                cancellationToken: cancellationToken);

            return new PaginatedList<T>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error {OperationName}", operationName);
            throw;
        }
    }
}
