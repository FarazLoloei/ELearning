using Dapr.Client;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.SharedKernel;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

public class EnrollmentReadService(DaprClient daprClient, ILogger<EnrollmentReadService> logger)
    : IEnrollmentReadService
{
    private const string StateStoreName = "enrollmentstore";

    public async Task<EnrollmentDetailDto> GetByIdAsync(Guid id)
    {
        try
        {
            var enrollment = await daprClient.GetStateAsync<EnrollmentDetailDto>(
                StateStoreName,
                id.ToString());

            if (enrollment == null)
            {
                throw new NotFoundException(nameof(Enrollment), id);
            }

            return enrollment;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving enrollment with ID {EnrollmentId}", id);
            throw;
        }
    }

    public async Task<PaginatedList<EnrollmentDetailDto>> ListAsync(int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<EnrollmentDetailDto>>(
                 httpMethod: HttpMethod.Get,
                "enrollmentservice",
                $"api/enrollments?pageNumber={pageNumber}&pageSize={pageSize}");

            return new PaginatedList<EnrollmentDetailDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error listing enrollments");
            throw;
        }
    }

    public async Task<PaginatedList<EnrollmentDto>> GetStudentEnrollmentsAsync(Guid studentId, int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<EnrollmentDto>>(
                 httpMethod: HttpMethod.Get,
                "enrollmentservice",
                $"api/students/{studentId}/enrollments?pageNumber={pageNumber}&pageSize={pageSize}");

            return new PaginatedList<EnrollmentDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting enrollments for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<PaginatedList<EnrollmentDto>> GetCourseEnrollmentsAsync(Guid courseId, int pageNumber, int pageSize)
    {
        try
        {
            var data = await daprClient.InvokeMethodAsync<PaginatedResponse<EnrollmentDto>>(
                 httpMethod: HttpMethod.Get,
                "enrollmentservice",
                $"api/courses/{courseId}/enrollments?pageNumber={pageNumber}&pageSize={pageSize}");

            return new PaginatedList<EnrollmentDto>(
                data.Items,
                data.TotalCount,
                pageNumber,
                pageSize);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting enrollments for course {CourseId}", courseId);
            throw;
        }
    }

    public async Task<EnrollmentDetailDto> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId)
    {
        try
        {
            var enrollment = await daprClient.InvokeMethodAsync<EnrollmentDetailDto>(
                 httpMethod: HttpMethod.Get,
                "enrollmentservice",
                $"api/students/{studentId}/courses/{courseId}/enrollment");

            if (enrollment == null)
            {
                throw new NotFoundException("Enrollment", $"Student: {studentId}, Course: {courseId}");
            }

            return enrollment;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            logger.LogError(ex, "Error retrieving enrollment for student {StudentId} and course {CourseId}", studentId, courseId);
            throw;
        }
    }
}