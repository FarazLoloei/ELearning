using Dapr.Client;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.Dtos;
using ELearning.Infrastructure.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;

namespace ELearning.Infrastructure.Data.Repositories;

public class ModuleReadRepository(DaprClient daprClient) : IModuleReadRepository
{
    private const string CourseStateStoreName = "coursestore";
    private const string CourseServiceAppId = "courseservice";
    private const int CourseDiscoveryPageSize = 100;

    public async Task<ModuleReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var modules = await LoadAllModulesAsync(cancellationToken);
        return modules.FirstOrDefault(m => m.Id == id);
    }

    public async Task<IReadOnlyList<ModuleReadModel>> GetByCourseIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        var course = await LoadCourseAsync(courseId, cancellationToken);
        return course?.Modules
            .OrderBy(m => m.Order)
            .ThenBy(m => m.Title)
            .Select(m => new ModuleReadModel(
                m.Id,
                m.Title,
                m.Description,
                m.Order,
                courseId))
            .ToList()
            ?? new List<ModuleReadModel>();
    }

    public async Task<PaginatedList<ModuleReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var modules = await LoadAllModulesAsync(cancellationToken);
        var items = modules
            .Skip(pagination.SkipCount)
            .Take(pagination.PageSize)
            .ToList();

        return new PaginatedList<ModuleReadModel>(
            items,
            modules.Count,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private async Task<List<ModuleReadModel>> LoadAllModulesAsync(CancellationToken cancellationToken)
    {
        var courseIds = await LoadAllCourseIdsAsync(cancellationToken);
        var modules = new List<ModuleReadModel>();

        foreach (var courseId in courseIds)
        {
            var course = await LoadCourseAsync(courseId, cancellationToken);
            if (course is null)
            {
                continue;
            }

            modules.AddRange(course.Modules.Select(m => new ModuleReadModel(
                m.Id,
                m.Title,
                m.Description,
                m.Order,
                courseId)));
        }

        return modules
            .OrderBy(m => m.CourseId)
            .ThenBy(m => m.Order)
            .ThenBy(m => m.Title)
            .ToList();
    }

    private async Task<List<Guid>> LoadAllCourseIdsAsync(CancellationToken cancellationToken)
    {
        var courseIds = new List<Guid>();
        var pageNumber = 1;
        var totalCount = int.MaxValue;

        while (courseIds.Count < totalCount)
        {
            var response = await daprClient.InvokeMethodAsync<PaginatedResponse<CourseListDto>>(
                HttpMethod.Get,
                CourseServiceAppId,
                $"api/courses?pageNumber={pageNumber}&pageSize={CourseDiscoveryPageSize}",
                cancellationToken);

            totalCount = response.TotalCount;
            if (response.Items.Count == 0)
            {
                break;
            }

            courseIds.AddRange(response.Items.Select(c => c.Id));
            pageNumber++;
        }

        return courseIds;
    }

    private async Task<CourseDto?> LoadCourseAsync(Guid courseId, CancellationToken cancellationToken)
    {
        return await daprClient.GetStateAsync<CourseDto>(
            CourseStateStoreName,
            courseId.ToString(),
            cancellationToken: cancellationToken);
    }
}
