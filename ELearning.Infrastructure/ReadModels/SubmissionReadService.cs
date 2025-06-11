using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Submissions.Abstractions.ReadModels;
using ELearning.Application.Submissions.Dtos;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class SubmissionReadService(DaprClient daprClient, ILogger<SubmissionReadService> logger) : ISubmissionReadService
{
    private const string StateStoreName = "submissionstore";
    private const string SubmissionServiceName = "submissionservice";

    public async Task<SubmissionDetailDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var submission = await daprClient.GetStateAsync<SubmissionDetailDto>(
                StateStoreName,
                id.ToString(),
                cancellationToken: cancellationToken);

            return submission == null ? throw new NotFoundException(nameof(Submission), id) : submission;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving submission with ID {SubmissionId}", id);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDetailDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken) =>
        await InvokePaginatedService<SubmissionDetailDto>(
            "api/submissions",
            "Error listing submissions",
            pagination,
            cancellationToken);

    public async Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(Guid instructorId, PaginationParameters pagination, CancellationToken cancellationToken) =>
        await InvokePaginatedService<SubmissionDto>(
            $"api/instructors/{instructorId}/pending-submissions",
            $"Error getting pending submissions for instructor {instructorId}",
            pagination,
            cancellationToken);

    public async Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, PaginationParameters pagination, CancellationToken cancellationToken) =>
        await InvokePaginatedService<SubmissionDto>(
            $"api/students/{studentId}/submissions",
            $"Error getting submissions for student {studentId}",
            pagination,
            cancellationToken);

    public async Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, PaginationParameters pagination, CancellationToken cancellationToken) =>
        await InvokePaginatedService<SubmissionDto>(
            $"api/assignments/{assignmentId}/submissions",
            $"Error getting submissions for assignment {assignmentId}",
            pagination,
            cancellationToken);

    private async Task<PaginatedList<T>> InvokePaginatedService<T>(
        string endpoint,
        string errorMessage,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<T>>(
            HttpMethod.Get,
            SubmissionServiceName,
            endpoint,
            cancellationToken);

            return new PaginatedList<T>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {ErrorMessage}", errorMessage);
            throw;
        }
    }
}