// <copyright file="AddCourseAssignmentCommandHandler.cs" company="FarazLoloei">
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
using ELearning.Domain.Entities.CourseAggregate.Exceptions;
using MediatR;

public sealed class AddCourseAssignmentCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<AddCourseAssignmentCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCourseAssignmentCommand request, CancellationToken cancellationToken)
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

        var assignmentType = AssignmentType.GetAll<AssignmentType>()
            .FirstOrDefault(type => type.Id == request.TypeId);

        if (assignmentType is null)
        {
            return Result.Failure<Guid>(ApplicationError.BadRequest($"Invalid assignment type: {request.TypeId}"));
        }

        Assignment assignment;
        try
        {
            assignment = new Assignment(
                request.Title,
                request.Description,
                assignmentType,
                request.MaxPoints,
                request.ModuleId,
                request.DueDate);

            course.AddAssignmentToModule(request.ModuleId, assignment);
        }
        catch (ModuleNotFoundException)
        {
            throw new NotFoundException(nameof(Module), request.ModuleId);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or ArgumentOutOfRangeException)
        {
            return Result.Failure<Guid>(ApplicationError.Conflict(ex.Message));
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success(assignment.Id);
    }
}
