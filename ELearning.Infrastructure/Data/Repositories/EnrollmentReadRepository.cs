// <copyright file="EnrollmentReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using System.Net.Http.Json;
using Dapr.Client;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.ReadModels;
using ELearning.Infrastructure.DaprServices;
using ELearning.Infrastructure.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;

/// <summary>
/// Reads enrollment projections through Dapr-backed state and service calls.
/// </summary>
public class EnrollmentReadRepository(DaprClient daprClient, IHttpClientFactory httpClientFactory) : IEnrollmentReadRepository
{
    private const string StateStoreName = "enrollmentstore";
    private const string EnrollmentServiceAppId = "enrollmentservice";

    /// <summary>
    /// Gets a single enrollment detail projection by identifier.
    /// </summary>
    /// <param name="id">The enrollment identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The enrollment detail projection when found; otherwise <see langword="null"/>.</returns>
    public async Task<EnrollmentDetailReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await daprClient.GetStateAsync<EnrollmentDetailDto>(
            StateStoreName,
            id.ToString(),
            cancellationToken: cancellationToken);

        return enrollment is null ? null : MapToDetailReadModel(enrollment);
    }

    /// <summary>
    /// Gets a paged list of enrollments for a specific student.
    /// </summary>
    /// <param name="studentId">The student identifier.</param>
    /// <param name="pagination">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of enrollment summary projections.</returns>
    public async Task<PaginatedList<EnrollmentSummaryReadModel>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var response = await this.InvokeGetAsync<PaginatedResponse<EnrollmentDto>>(
            $"api/students/{studentId}/enrollments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
            cancellationToken);

        return new PaginatedList<EnrollmentSummaryReadModel>(
            [.. response.Items.Select(MapToSummaryReadModel)],
            response.TotalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    /// <summary>
    /// Gets a paged list of enrollment detail projections.
    /// </summary>
    /// <param name="pagination">The paging parameters.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of enrollment detail projections.</returns>
    public async Task<PaginatedList<EnrollmentDetailReadModel>> ListAsync(
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var response = await this.InvokeGetAsync<PaginatedResponse<EnrollmentDetailDto>>(
            $"api/enrollments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
            cancellationToken);

        return new PaginatedList<EnrollmentDetailReadModel>(
            [.. response.Items.Select(MapToDetailReadModel)],
            response.TotalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    private static EnrollmentDetailReadModel MapToDetailReadModel(EnrollmentDetailDto dto) =>
        new(
            dto.Id,
            dto.StudentId,
            dto.StudentName,
            dto.CourseId,
            dto.CourseTitle,
            dto.Status,
            dto.EnrollmentDate,
            dto.CompletedDate,
            dto.CompletionPercentage,
            [.. dto.LessonProgress.Select(p => new LessonProgressReadModel(p.LessonId, p.LessonTitle, p.Status, p.CompletedDate, p.TimeSpentSeconds))],
            [.. dto.Submissions.Select(s => new EnrollmentSubmissionReadModel(s.Id, s.AssignmentId, s.AssignmentTitle, s.SubmittedDate, s.IsGraded, s.Score, s.MaxPoints))],
            dto.CourseRating,
            dto.Review);

    private static EnrollmentSummaryReadModel MapToSummaryReadModel(EnrollmentDto dto) =>
        new(
            dto.Id,
            dto.StudentId,
            dto.StudentName,
            dto.CourseId,
            dto.CourseTitle,
            dto.Status,
            dto.EnrollmentDate,
            dto.CompletedDate,
            dto.CompletionPercentage);

    private async Task<TResponse> InvokeGetAsync<TResponse>(string methodPath, CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient(DaprConfig.SidecarHttpClientName);

        using var response = await httpClient.GetAsync(
            $"{EnrollmentServiceAppId}/method/{methodPath}",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException(
                $"Dapr service invocation returned no content for '{EnrollmentServiceAppId}/{methodPath}'.");
    }
}