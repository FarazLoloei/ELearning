// <copyright file="CurrentUserAuthorizationGuard.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Security;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;

public static class CurrentUserAuthorizationGuard
{
    public static void EnsureAuthenticated(ICurrentUserService currentUserService)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }
    }

    public static void EnsureStudentSelfOrAdmin(ICurrentUserService currentUserService, Guid requestedStudentId)
    {
        EnsureAuthenticated(currentUserService);

        var currentUserId = currentUserService.UserId!.Value;
        if (currentUserId != requestedStudentId && !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }
    }

    public static async Task EnsureEnrollmentReadAccessAsync(
        ICurrentUserService currentUserService,
        Guid studentId,
        Guid courseId,
        ICourseRepository courseRepository,
        CancellationToken cancellationToken)
    {
        EnsureAuthenticated(currentUserService);

        var currentUserId = currentUserService.UserId!.Value;
        if (currentUserId == studentId || currentUserService.IsInRole("Admin"))
        {
            return;
        }

        var course = await courseRepository.GetByIdForUpdateAsync(courseId, cancellationToken)
            ?? throw new NotFoundException("Course", courseId);

        if (course.InstructorId != currentUserId)
        {
            throw new ForbiddenAccessException();
        }
    }
}
