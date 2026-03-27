// <copyright file="LessonReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Net.Http.Json;
using Dapr.Client;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.ReadModels;
using ELearning.Infrastructure.DaprServices;
using ELearning.Infrastructure.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;

public class LessonReadRepository(DaprClient daprClient, IHttpClientFactory httpClientFactory) : ILessonReadRepository
{
    private const string CourseStateStoreName = "coursestore";
    private const string CourseServiceAppId = "courseservice";
    private const int CourseDiscoveryPageSize = 100;

    public async Task<LessonReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var lessons = await this.LoadAllLessonsAsync(cancellationToken);
        return lessons.FirstOrDefault(l => l.Id == id);
    }

    public async Task<IReadOnlyList<LessonReadModel>> GetByModuleIdAsync(Guid moduleId, CancellationToken cancellationToken = default)
    {
        var lessons = await this.LoadAllLessonsAsync(cancellationToken);
        return [.. lessons
            .Where(l => l.ModuleId == moduleId)
            .OrderBy(l => l.Order)
            .ThenBy(l => l.Title)];
    }

    public async Task<PaginatedList<LessonReadModel>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default)
    {
        var lessons = await this.LoadAllLessonsAsync(cancellationToken);
        var items = lessons
            .Skip(pagination.SkipCount)
            .Take(pagination.PageSize)
            .ToList();

        return new PaginatedList<LessonReadModel>(
            items,
            lessons.Count,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private async Task<List<LessonReadModel>> LoadAllLessonsAsync(CancellationToken cancellationToken)
    {
        var courseIds = await this.LoadAllCourseIdsAsync(cancellationToken);
        var lessons = new List<LessonReadModel>();

        foreach (var courseId in courseIds)
        {
            var course = await this.LoadCourseAsync(courseId, cancellationToken);
            if (course is null)
            {
                continue;
            }

            foreach (var module in course.Modules)
            {
                lessons.AddRange(module.Lessons.Select(lesson => new LessonReadModel(
                    lesson.Id,
                    lesson.Title,
                    module.Id,
                    lesson.Order)));
            }
        }

        return [.. lessons
            .OrderBy(l => l.ModuleId)
            .ThenBy(l => l.Order)
            .ThenBy(l => l.Title)];
    }

    private async Task<List<Guid>> LoadAllCourseIdsAsync(CancellationToken cancellationToken)
    {
        var courseIds = new List<Guid>();
        var pageNumber = 1;
        var totalCount = int.MaxValue;

        while (courseIds.Count < totalCount)
        {
            var response = await this.InvokeGetAsync<PaginatedResponse<CourseListDto>>(
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

    private async Task<TResponse> InvokeGetAsync<TResponse>(string methodPath, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(DaprConfig.SidecarHttpClientName);

        using var response = await httpClient.GetAsync(
            $"{CourseServiceAppId}/method/{methodPath}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                $"Dapr service invocation returned no content for '{CourseServiceAppId}/{methodPath}'.");
    }
}