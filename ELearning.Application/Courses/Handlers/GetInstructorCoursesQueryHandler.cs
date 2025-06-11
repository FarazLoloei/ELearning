using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
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
        catch (Exception)
        {
            // Fall back to repository
            var instructor = await instructorRepository.GetByIdAsync(request.InstructorId) ??
                throw new NotFoundException(nameof(Instructor), request.InstructorId);

            // Get instructor statistics
            var totalStudents = await instructorRepository.GetTotalStudentCountAsync(request.InstructorId, cancellationToken);
            var averageRating = await instructorRepository.GetAverageRatingAsync(request.InstructorId, cancellationToken);

            // Map to DTO
            var instructorCoursesDto = mapper.Map<InstructorCoursesDto>(instructor);
            //instructorCoursesDto.TotalStudents = totalStudents;
            //instructorCoursesDto.AverageRating = averageRating;
            //instructorCoursesDto.TotalCourses = instructor.Courses.Count;
            //instructorCoursesDto.Courses = new List<InstructorCourseDto>();

            // Get course details
            foreach (var course in instructor.Courses)
            {
                var enrollments = await enrollmentRepository.GetByCourseIdAsync(course.Id, cancellationToken);

                var courseDto = new InstructorCourseDto
                {
                    Id = course.Id,
                    Title = course.Title,
                    Category = course.Category.Name,
                    Status = course.Status.Name,
                    EnrollmentsCount = enrollments.Count,
                    CreatedAt = course.CreatedAt(),
                    PublishedDate = course.PublishedDate
                };

                instructorCoursesDto.Courses.Add(courseDto);
            }

            return Result.Success(instructorCoursesDto);
        }
    }
}