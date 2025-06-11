using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Students.Abstractions.ReadModels;
using ELearning.Application.Students.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class StudentReadService(DaprClient daprClient, ILogger<StudentReadService> logger) : IStudentReadService
{
    private const string StateStoreName = "userstore";

    public async Task<StudentDto> GetByIdAsync(Guid id)
    {
        try
        {
            var student = await daprClient.GetStateAsync<StudentDto>(
                StateStoreName,
                id.ToString());

            if (student == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            return student;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentDto> GetStudentByIdAsync(Guid id)
    {
        try
        {
            var student = await daprClient.InvokeMethodAsync<StudentDto>(
                HttpMethod.Get,
                "userservice",
                $"api/students/{id}");

            if (student == null)
            {
                throw new NotFoundException(nameof(Student), id);
            }

            return student;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
            throw;
        }
    }

    public async Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId)
    {
        try
        {
            var progress = await daprClient.InvokeMethodAsync<StudentProgressDto>(
                 httpMethod: HttpMethod.Get,
                "userservice",
                $"api/students/{studentId}/progress");

            if (progress == null)
            {
                throw new NotFoundException("Student progress", studentId);
            }

            return progress;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving progress for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<PaginatedList<StudentDto>> ListAsync(PaginationParameters pagination)
    {
        try
        {
            // In a real implementation, you would use Dapr queries or bulk get operations
            // For simplicity, we're simulating with a direct state get
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<StudentDto>>(
                httpMethod: HttpMethod.Get,
                "userservice",
                $"api/courses?pageNumber={pagination.PageNumber} &pageSize= {pagination.PageSize}");

            return new PaginatedList<StudentDto>(
                data.Items,
                data.TotalCount,
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