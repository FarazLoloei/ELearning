// <copyright file="DeleteCourseCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using MediatR;

/// <summary>
/// Handler for DeleteCourseCommand.
/// </summary>
public class DeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<DeleteCourseCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        var isInstructorOwner = course.IsOwnedBy(currentUserService.UserId.Value);

        if (!isInstructorOwner && !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        try
        {
            course.EnsureCanBeDeleted();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await courseRepository.DeleteAsync(course, cancellationToken);

        return Result.Success();
    }
}
