// <copyright file="ArchiveCourseCommandHandler.cs" company="FarazLoloei">
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

public sealed class ArchiveCourseCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<ArchiveCourseCommand, Result>
{
    public async Task<Result> Handle(ArchiveCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        try
        {
            course.Archive();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success();
    }
}
