using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Infrastructure.Dapr.Abstraction;
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

    public async Task<PaginatedList<SubmissionDetailDto>> ListAsync(int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDetailDto>>(
                "submissionservice",
                $"api/submissions?pageNumber={pageNumber}&pageSize={pageSize}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            return new PaginatedList<SubmissionDetailDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing submissions");
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetPendingSubmissionsAsync(Guid instructorId, int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                "submissionservice",
                $"api/instructors/{instructorId}/pending-submissions?pageNumber={pageNumber}&pageSize={pageSize}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting pending submissions for instructor {InstructorId}", instructorId);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetStudentSubmissionsAsync(Guid studentId, int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                "submissionservice",
                $"api/students/{studentId}/submissions?pageNumber={pageNumber}&pageSize={pageSize}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting submissions for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<PaginatedList<SubmissionDto>> GetAssignmentSubmissionsAsync(Guid assignmentId, int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<SubmissionDto>>(
                "submissionservice",
                $"api/assignments/{assignmentId}/submissions?pageNumber={pageNumber}&pageSize={pageSize}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            return new PaginatedList<SubmissionDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting submissions for assignment {AssignmentId}", assignmentId);
            throw;
        }
    }
}