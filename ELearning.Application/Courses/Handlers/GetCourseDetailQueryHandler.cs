// <copyright file="GetCourseDetailQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using AutoMapper;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Dtos;
using ELearning.Application.Courses.Queries;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using MediatR;

public class GetCourseDetailQueryHandler(
        ICourseRepository courseRepository,
        IMapper mapper,
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

        var courseDto = mapper.Map<CourseDto>(course);

        return Result.Success(courseDto);
    }
}
