// <copyright file="UpdateEnrollmentStatusCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Commands;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using MediatR;

/// <summary>
/// Handler for UpdateEnrollmentStatusCommand.
/// </summary>
public class UpdateEnrollmentStatusCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<UpdateEnrollmentStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateEnrollmentStatusCommand request, CancellationToken cancellationToken)
    {
        this.EnsureAuthenticated();

        var enrollment = await enrollmentRepository.GetByIdForUpdateAsync(request.EnrollmentId, cancellationToken)
                          ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        this.EnsureAuthorized(enrollment);

        try
        {
            var status = request.Status.Trim();

            if (status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                enrollment.Resume();
            }
            else if (status.Equals("Paused", StringComparison.OrdinalIgnoreCase))
            {
                enrollment.Pause();
            }
            else if (status.Equals("Abandoned", StringComparison.OrdinalIgnoreCase))
            {
                enrollment.Abandon();
            }
            else if (status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            {
                return Result.Failure("Course completion is driven by lesson progression and cannot be set manually.");
            }
            else
            {
                return Result.Failure("Invalid status value.");
            }
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Result.Success();
    }

    private void EnsureAuthenticated()
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }
    }

    private void EnsureAuthorized(Enrollment enrollment)
    {
        var isOwner = enrollment.StudentId == currentUserService.UserId;
        var isAuthorized = isOwner || currentUserService.IsInRole("Admin");

        if (!isAuthorized)
        {
            throw new ForbiddenAccessException();
        }
    }
}
