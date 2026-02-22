using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

/// <summary>
/// Handler for GetInstructorCoursesQuery
/// </summary>
public class GetInstructorCoursesQueryHandler(
        IInstructorRepository instructorRepository,
        IEnrollmentRepository enrollmentRepository,
        IMapper mapper)
    : IRequestHandler<GetInstructorCoursesQuery, Result<InstructorCoursesDto>>
{
    public async Task<Result<InstructorCoursesDto>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Try Dapr read service first
            var instructor = await instructorRepository.GetInstructorWithCoursesAsync(request.InstructorId, cancellationToken);
            var instructorCoursesDto = mapper.Map<InstructorCoursesDto>(instructor);
            return Result.Success(instructorCoursesDto);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // Fall back to repository
            var instructor = await instructorRepository.GetByIdAsync(request.InstructorId) ??
                throw new NotFoundException(nameof(Instructor), request.InstructorId);

            var mappedInstructorCoursesDto = mapper.Map<InstructorCoursesDto>(instructor);

            // Get course details
            var courses = new List<InstructorCourseDto>();
            foreach (var course in instructor.Courses)
            {
                var enrollments = await enrollmentRepository.GetByCourseIdAsync(course.Id, cancellationToken);

                var courseDto = new InstructorCourseDto(
                    course.Id,
                    course.Title,
                    course.Category.Name,
                    enrollments.Count,
                    course.Status.Name,
                    course.CreatedAt(),
                    course.PublishedDate);

                courses.Add(courseDto);
            }

            var instructorCoursesDto = mappedInstructorCoursesDto with { Courses = courses };
            return Result.Success(instructorCoursesDto);
        }
    }
}

