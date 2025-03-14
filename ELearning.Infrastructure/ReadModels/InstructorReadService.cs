using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Instructors.Abstractions.ReadModels;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.SharedKernel;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class InstructorReadService(DaprClient daprClient, ILogger<InstructorReadService> logger) : IInstructorReadService
{
    private const string StateStoreName = "userstore";

    public async Task<InstructorDto> GetByIdAsync(Guid id)
    {
        try
        {
            var instructor = await daprClient.GetStateAsync<InstructorDto>(
                StateStoreName,
                id.ToString());

            if (instructor == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            return instructor;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving instructor with ID {InstructorId}", id);
            throw;
        }
    }

    public async Task<InstructorDto> GetInstructorByIdAsync(Guid id)
    {
        try
        {
            var instructor = await daprClient.InvokeMethodAsync<InstructorDto>(
                httpMethod: HttpMethod.Get,
                "userservice",
                $"api/instructors/{id}");

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

    public async Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId)
    {
        try
        {
            var instructorWithCourses = await daprClient.InvokeMethodAsync<InstructorCoursesDto>(
                 httpMethod: HttpMethod.Get,
                "userservice",
                $"api/instructors/{instructorId}/with-courses");

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

    public async Task<PaginatedList<InstructorDto>> ListAsync(int pageNumber, int pageSize)
    {
        try
        {
            // In a real implementation, you would use Dapr queries or bulk get operations
            // For simplicity, we're simulating with a direct state get
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<InstructorDto>>(
                httpMethod: HttpMethod.Get,
                StateStoreName,
                $"api/courses?pageNumber={pageNumber}&pageSize={pageSize}");

            return new PaginatedList<InstructorDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing instructors");
            throw;
        }
    }
}