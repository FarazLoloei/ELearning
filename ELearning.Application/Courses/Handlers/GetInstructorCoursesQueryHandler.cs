// <copyright file="GetInstructorCoursesQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

/// <summary>
/// Handler for GetInstructorCoursesQuery.
/// </summary>
public class GetInstructorCoursesQueryHandler(
        IInstructorReadRepository instructorReadRepository)
    : IRequestHandler<GetInstructorCoursesQuery, Result<InstructorCoursesDto>>
{
    public async Task<Result<InstructorCoursesDto>> Handle(GetInstructorCoursesQuery request, CancellationToken cancellationToken)
    {
        var instructor = await instructorReadRepository.GetInstructorWithCoursesAsync(request.InstructorId, cancellationToken)
            ?? throw new NotFoundException("Instructor", request.InstructorId);

        var instructorCoursesDto = new InstructorCoursesDto(
            instructor.Id,
            instructor.FullName,
            instructor.Email,
            instructor.Bio,
            instructor.Expertise,
            instructor.ProfilePictureUrl ?? string.Empty,
            instructor.AverageRating,
            instructor.TotalStudents,
            instructor.TotalCourses,
            instructor.Courses.Select(course => new InstructorCourseDto(
                course.Id,
                course.Title,
                CourseCategory.GetAll<CourseCategory>().FirstOrDefault(c => c.Id == course.CategoryId)?.Name ?? "Unknown",
                course.EnrollmentsCount,
                CourseStatus.GetAll<CourseStatus>().FirstOrDefault(s => s.Id == course.StatusId)?.Name ?? "Unknown",
                course.CreatedAtUtc,
                course.PublishedDate)).ToList());

        return Result.Success(instructorCoursesDto);
    }
}
