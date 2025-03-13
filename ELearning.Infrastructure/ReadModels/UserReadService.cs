using ELearning.Application.Common.Exceptions;
using ELearning.Application.Instructors.Dtos;
using ELearning.Application.Students;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Infrastructure.Dapr.Abstraction;
using Microsoft.Extensions.Logging;

namespace ELearning.Infrastructure.ReadModels;

// Implementation of User read service using Dapr
public class UserReadService : IUserReadService
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<UserReadService> _logger;
    private const string StateStoreName = "userstore";

    public UserReadService(DaprClient daprClient, ILogger<UserReadService> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public async Task<StudentDto> GetStudentByIdAsync(Guid id)
    {
        try
        {
            var student = await _daprClient.InvokeMethodAsync<StudentDto>(
                "userservice",
                $"api/students/{id}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            if (student == null)
            {
                throw new NotFoundException(nameof(Student), id);
            }

            return student;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving student with ID {StudentId}", id);
            throw;
        }
    }

    public async Task<InstructorDto> GetInstructorByIdAsync(Guid id)
    {
        try
        {
            var instructor = await _daprClient.InvokeMethodAsync<InstructorDto>(
                "userservice",
                $"api/instructors/{id}",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            if (instructor == null)
            {
                throw new NotFoundException(nameof(Instructor), id);
            }

            return instructor;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving instructor with ID {InstructorId}", id);
            throw;
        }
    }

    public async Task<StudentProgressDto> GetStudentProgressAsync(Guid studentId)
    {
        try
        {
            var progress = await _daprClient.InvokeMethodAsync<StudentProgressDto>(
                "userservice",
                $"api/students/{studentId}/progress",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            if (progress == null)
            {
                throw new NotFoundException("Student progress", studentId);
            }

            return progress;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving progress for student {StudentId}", studentId);
            throw;
        }
    }

    public async Task<InstructorCoursesDto> GetInstructorWithCoursesAsync(Guid instructorId)
    {
        try
        {
            var instructorWithCourses = await _daprClient.InvokeMethodAsync<InstructorCoursesDto>(
                "userservice",
                $"api/instructors/{instructorId}/with-courses",
                new HttpInvocationOptions { Method = HttpMethod.Get });

            if (instructorWithCourses == null)
            {
                throw new NotFoundException(nameof(Instructor), instructorId);
            }

            return instructorWithCourses;
        }
        catch (Exception ex) when (ex is not NotFoundException)
        {
            _logger.LogError(ex, "Error retrieving instructor {InstructorId} with courses", instructorId);
            throw;
        }
    }
}