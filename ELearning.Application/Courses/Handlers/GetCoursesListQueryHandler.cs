// <copyright file="GetCoursesListQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Abstractions;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Application.Courses.ReadModels;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using ELearning.SharedKernel;
using MediatR;

public class GetCoursesListQueryHandler(
        ICourseReadRepository courseReadRepository)
    : IRequestHandler<GetCoursesListQuery, Result<PaginatedList<CourseListDto>>>
{
    public async Task<Result<PaginatedList<CourseListDto>>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken)
    {
        var courses = await courseReadRepository.SearchAsync(
            request.SearchTerm,
            request.CategoryId,
            request.LevelId,
            request.IsFeatured,
            request.ToPaginationParameters(),
            cancellationToken);

        var courseDtos = courses.Items
            .Select(MapToDto)
            .ToList();

        var paginatedList = new PaginatedList<CourseListDto>(
            courseDtos,
            courses.TotalCount,
            request.PageNumber,
            request.PageSize);

        return Result.Success(paginatedList);
    }

    private static CourseListDto MapToDto(CourseReadModel course)
    {
        var categoryName = CourseCategory.GetAll<CourseCategory>()
            .FirstOrDefault(category => category.Id == course.CategoryId)?.Name ?? "Unknown";

        var levelName = CourseLevel.GetAll<CourseLevel>()
            .FirstOrDefault(level => level.Id == course.LevelId)?.Name ?? "Unknown";

        var duration = Duration.Create(course.DurationHours, course.DurationMinutes).ToString();

        return new CourseListDto(
            course.Id,
            course.Title,
            course.Description,
            course.InstructorName,
            categoryName,
            levelName,
            course.Price,
            course.AverageRating,
            course.NumberOfRatings,
            course.IsFeatured,
            duration,
            course.EnrollmentsCount);
    }
}
