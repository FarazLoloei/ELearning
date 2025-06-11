using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Dtos;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;
using System.Web;

namespace ELearning.Infrastructure.ReadModels;

public class StudentReadService(DaprClient daprClient, ILogger<StudentReadService> logger) : IStudentReadService
{
    private const string StateStoreName = "userstore";
    private const string UserServiceName = "userservice";

    public async Task<StudentDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var student = await daprClient.GetStateAsync<StudentDto>(
                StateStoreName,
                id.ToString(),
                cancellationToken: cancellationToken);

            return (student == null) ? throw new NotFoundException(nameof(User), id) : student;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving student state for ID {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentDto> GetStudentByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var student = await daprClient.InvokeMethodAsync<StudentDto>(
                HttpMethod.Get,
                UserServiceName,
                $"api/students/{id}",
                cancellationToken);

            return (student == null) ? throw new NotFoundException(nameof(User), id) : student;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving student from service for ID {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId, CancellationToken cancellationToken)
    {
        try
        {
            var progress = await daprClient.InvokeMethodAsync<StudentProgressDto>(
                HttpMethod.Get,
                UserServiceName,
                $"api/students/{studentId}/progress",
                cancellationToken);

            return (progress == null) ? throw new NotFoundException("Student progress", studentId) : progress;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving progress for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<PaginatedList<StudentDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        try
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pagination.PageNumber.ToString();
            queryParams["pageSize"] = pagination.PageSize.ToString();

            var endpoint = $"api/students?{queryParams}";

            var response = await daprClient.InvokeMethodAsync<PaginatedResponse<StudentDto>>(
                HttpMethod.Get,
                UserServiceName,
                endpoint,
                cancellationToken);

            return new PaginatedList<StudentDto>(
                response.Items,
                response.TotalCount,
                pagination.PageNumber,
                pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing students");
            throw;
        }
    }
}