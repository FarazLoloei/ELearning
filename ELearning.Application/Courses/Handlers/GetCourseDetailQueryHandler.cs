// <copyright file="GetCourseDetailQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Instructors.Dtos;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public class GetCourseDetailQueryHandler(
        ICourseRepository courseRepository,
        ICourseReadRepository courseReadRepository,
        IInstructorReadRepository instructorReadRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetCourseDetailQuery, Result<CourseDto>>
{
    public async Task<Result<CourseDto>> Handle(GetCourseDetailQuery request, CancellationToken cancellationToken)
    {
        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        var canViewUnpublishedCourse =
            currentUserService.IsAuthenticated &&
            currentUserService.UserId is Guid currentUserId &&
            (course.IsOwnedBy(currentUserId) || currentUserService.IsInRole("Admin"));

        if (!course.IsPubliclyVisible() && !canViewUnpublishedCourse)
        {
            throw new NotFoundException(nameof(Course), request.CourseId);
        }

        var instructor = await instructorReadRepository.GetByIdAsync(course.InstructorId, cancellationToken)
            ?? throw new NotFoundException("Instructor", course.InstructorId);

        var reviews = await courseReadRepository.GetReviewsByCourseIdAsync(request.CourseId, cancellationToken);

        var courseDto = new CourseDto(
            course.Id,
            course.Title,
            course.Description,
            new InstructorDto(
                instructor.Id,
                instructor.FullName,
                instructor.Email,
                instructor.Bio,
                instructor.Expertise,
                instructor.ProfilePictureUrl ?? string.Empty,
                instructor.AverageRating,
                instructor.TotalStudents,
                instructor.TotalCourses),
            course.Status.Name,
            course.Category.Name,
            course.Level.Name,
            course.Price,
            course.Duration.ToString(),
            course.PublishedDate,
            course.AverageRating.Value,
            course.AverageRating.NumberOfRatings,
            [.. course.Modules
                .OrderBy(module => module.Order)
                .Select(module => new ModuleDto(
                    module.Id,
                    module.Title,
                    module.Description,
                    module.Order,
                    [.. module.Lessons
                        .OrderBy(lesson => lesson.Order)
                        .Select(lesson => new LessonDto(
                            lesson.Id,
                            lesson.Title,
                            lesson.Content,
                            lesson.Type.Name,
                            lesson.VideoUrl ?? string.Empty,
                            lesson.Duration.ToString(),
                            lesson.Order))],
                    [.. module.Assignments
                        .Select(assignment => new AssignmentDto(
                            assignment.Id,
                            assignment.Title,
                            assignment.Description,
                            assignment.Type.Name,
                            assignment.MaxPoints,
                            assignment.DueDate))]))],
            [.. reviews.Select(review => new ReviewDto(
                review.Id,
                review.StudentName,
                review.Rating,
                review.Comment,
                review.CreatedAt))]);

        return Result.Success(courseDto);
    }
}
