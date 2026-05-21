// <copyright file="AddCourseModuleCommandHandler.cs" company="FarazLoloei">
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

public sealed class AddCourseModuleCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<AddCourseModuleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCourseModuleCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        if (!course.IsOwnedBy(currentUserService.UserId.Value))
        {
            throw new ForbiddenAccessException();
        }

        Module module;
        try
        {
            module = new Module(request.Title, request.Description, request.Order, course.Id);
            course.AddModule(module);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return Result.Failure<Guid>(ApplicationError.Conflict(ex.Message));
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success(module.Id);
    }
}
