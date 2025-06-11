using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Submissions.Abstractions.ReadModels;
using ELearning.Application.Submissions.Dtos;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class SubmissionReadService(DaprClient daprClient, ILogger<SubmissionReadService> logger)
    : ISubmissionReadService
{
    private const string StateStoreName = "submissionstore";

    public async Task<SubmissionDetailDto> GetByIdAsync(Guid id)
    {
        try
        {
            var submission = await daprClient.GetStateAsync<SubmissionDetailDto>(
                StateStoreName,
                id.ToString());

            if (submission == null)
            {
                throw new NotFoundException(nameof(Submission), id);
            }

            return submission;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving submission with ID {SubmissionId}", id);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDetailDto>> ListAsync(PaginationParameters pagination)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDetailDto>>(
                httpMethod: HttpMethod.Get,
                "submissionservice",
                $"api/submissions?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}");

            return new PaginatedList<SubmissionDetailDto>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing submissions");
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(Guid instructorId, PaginationParameters pagination)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                httpMethod: HttpMethod.Get,
                "submissionservice",
                $"api/instructors/{instructorId}/pending-submissions?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}");

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting pending submissions for instructor {InstructorId}", instructorId);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, PaginationParameters pagination)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                httpMethod: HttpMethod.Get,
                "submissionservice",
                $"api/students/{studentId}/submissions?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}");

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                 pagination.PageNumber,
                 pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting submissions for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, PaginationParameters pagination)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                httpMethod: HttpMethod.Get,
                "submissionservice",
                $"api/assignments/{assignmentId}/submissions?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}");

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting submissions for assignment {AssignmentId}", assignmentId);
            throw;
        }
    }
}