using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Instructors.Abstractions.ReadModels;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class InstructorReadService(DaprClient daprClient, ILogger<InstructorReadService> logger) : IInstructorReadService
{
    private const string StateStoreName = "userstore";
    private const string UserServiceName = "userservice";

    public async Task<InstructorDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var instructor = await daprClient.GetStateAsync<InstructorDto>(
                StateStoreName,
                id.ToString(),
                cancellationToken: cancellationToken);

            if (instructor == null)
            {
                throw new NotFoundException(nameof(Instructor), id);
            }

            return instructor;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving instructor with ID {InstructorId}", id);
            throw;
        }
    }

    public async Task<InstructorDto> GetInstructorByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var instructor = await daprClient.InvokeMethodAsync<InstructorDto>(
                httpMethod: HttpMethod.Get,
                UserServiceName,
                $"api/instructors/{id}",
                cancellationToken: cancellationToken);

            if (instructor == null)
            {
                throw new NotFoundException(nameof(Instructor), id);
            }

            return instructor;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving instructor with ID {InstructorId}", id);
            throw;
        }
    }

    public async Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId, CancellationToken cancellationToken)
    {
        try
        {
            var instructorWithCourses = await daprClient.InvokeMethodAsync<InstructorCoursesDto>(
                 httpMethod: HttpMethod.Get,
                UserServiceName,
                $"api/instructors/{instructorId}/with-courses",
                cancellationToken: cancellationToken);

            if (instructorWithCourses == null)
            {
                throw new NotFoundException(nameof(Instructor), instructorId);
            }

            return instructorWithCourses;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving instructor {InstructorId} with courses", instructorId);
            throw;
        }
    }

    public async Task<PaginatedList<InstructorDto>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken)
    {
        try
        {
            // In a real implementation, you would use Dapr queries or bulk get operations
            // For simplicity, we're simulating with a direct state get
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<InstructorDto>>(
                httpMethod: HttpMethod.Get,
                UserServiceName,
                $"api/instructors?pageNumber={pagination.PageNumber}&pageSize={pagination.PageSize}",
                cancellationToken: cancellationToken);

            return new PaginatedList<InstructorDto>(
                data.Items,
                data.TotalCount,
                pagination.PageNumber, pagination.PageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing instructors");
            throw;
        }
    }
}
