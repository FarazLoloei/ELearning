using Dapr.Client;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.ReadModels;
using ELearning.Infrastructure.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;

namespace ELearning.Infrastructure.Data.Repositories;

public class EnrollmentReadRepository(DaprClient daprClient) : IEnrollmentReadRepository
{
    private const string StateStoreName = "enrollmentstore";
    private const string EnrollmentServiceAppId = "enrollmentservice";

    public async Task<EnrollmentDetailReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var enrollment = await daprClient.GetStateAsync<EnrollmentDetailDto>(
            StateStoreName,
            id.ToString(),
            cancellationToken: cancellationToken);

        return enrollment is null ? null : MapToDetailReadModel(enrollment);
    }

    public async Task<PaginatedList<EnrollmentSummaryReadModel>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var response = await daprClient.InvokeMethodAsync<PaginatedResponse<EnrollmentDto>>(
            HttpMethod.Get,
            EnrollmentServiceAppId,
            $"api/students/{studentId}/enrollments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
            cancellationToken);

        return new PaginatedList<EnrollmentSummaryReadModel>(
            response.Items.Select(MapToSummaryReadModel).ToList(),
            response.TotalCount,
            pagination.PageNumber,
            pagination.PageSize);
    }

    public async Task<PaginatedList<EnrollmentDetailReadModel>> ListAsync(
        PaginationParameters pagination,
        CancellationToken cancellationToken = default)
    {
        var response = await daprClient.InvokeMethodAsync<PaginatedResponse<EnrollmentDetailDto>>(
            HttpMethod.Get,
            EnrollmentServiceAppId,
            $"api/enrollments?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
            cancellationToken);

        return new PaginatedList<EnrollmentDetailReadModel>(
            response.Items.Select(MapToDetailReadModel).ToList(),
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
            dto.LessonProgress
                .Select(p => new LessonProgressReadModel(
                    p.LessonId,
                    p.LessonTitle,
                    p.Status,
                    p.CompletedDate,
                    p.TimeSpentSeconds))
                .ToList(),
            dto.Submissions
                .Select(s => new EnrollmentSubmissionReadModel(
                    s.Id,
                    s.AssignmentId,
                    s.AssignmentTitle,
                    s.SubmittedDate,
                    s.IsGraded,
                    s.Score,
                    s.MaxPoints))
                .ToList(),
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
}
