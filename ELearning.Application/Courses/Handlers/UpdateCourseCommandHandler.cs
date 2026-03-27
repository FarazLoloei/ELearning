// <copyright file="UpdateCourseCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.ValueObjects;
using MediatR;

/// <summary>
/// Handler for UpdateCourseCommand.
/// </summary>
public class UpdateCourseCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<UpdateCourseCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        var isInstructorOwner = course.IsOwnedBy(currentUserService.UserId.Value);

        if (!isInstructorOwner)
        {
            throw new ForbiddenAccessException();
        }

        var category = CourseCategory.GetAll<CourseCategory>()
            .FirstOrDefault(c => c.Id == request.CategoryId);

        var level = CourseLevel.GetAll<CourseLevel>()
            .FirstOrDefault(l => l.Id == request.LevelId);

        if (category is null || level is null)
        {
            return Result.Failure($"Invalid category or level. Category: {category?.Name ?? "null"}, Level: {level?.Name ?? "null"}");
        }

        try
        {
            var duration = Duration.Create(request.DurationHours, request.DurationMinutes);

            course.UpdateDetails(request.Title, request.Description, category, level, duration);
            course.UpdatePrice(request.Price);

            if (course.IsFeatured != request.IsFeatured)
            {
                course.ToggleFeatured();
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return Result.Failure(ex.Message);
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success();
    }
}
