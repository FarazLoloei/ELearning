using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Enrollments.Abstractions.ReadModels;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

/// <summary>
/// Handler for GetInstructorCoursesQuery
/// </summary>
public class GetInstructorCoursesQueryHandler(
        IInstructorRepository instructorRepository,
        IEnrollmentReadRepository enrollmentReadRepository,
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
            var instructor = await instructorRepository.GetByIdAsync(request.InstructorId, cancellationToken) ??
                throw new NotFoundException(nameof(Instructor), request.InstructorId);

            var mappedInstructorCoursesDto = mapper.Map<InstructorCoursesDto>(instructor);

            var courseIds = instructor.Courses.Select(course => course.Id).ToList();
            var enrollmentCountsByCourseId = await enrollmentReadRepository.GetCourseEnrollmentCountsAsync(courseIds, cancellationToken);

            // Get course details
            var courses = new List<InstructorCourseDto>();
            foreach (var course in instructor.Courses)
            {
                enrollmentCountsByCourseId.TryGetValue(course.Id, out var enrollmentCount);

                var courseDto = new InstructorCourseDto(
                    course.Id,
                    course.Title,
                    course.Category.Name,
                    enrollmentCount,
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

