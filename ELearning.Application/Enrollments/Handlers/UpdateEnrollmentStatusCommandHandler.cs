using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Commands;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using MediatR;

namespace ELearning.Application.Enrollments.Handlers;

/// <summary>
/// Handler for UpdateEnrollmentStatusCommand.
/// </summary>
public class UpdateEnrollmentStatusCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<UpdateEnrollmentStatusCommand, Result>
{
    private static readonly Dictionary<string, EnrollmentStatus> StatusMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "Active", EnrollmentStatus.Active },
        { "Paused", EnrollmentStatus.Paused },
        { "Completed", EnrollmentStatus.Completed },
        { "Abandoned", EnrollmentStatus.Abandoned }
    };

    public async Task<Result> Handle(UpdateEnrollmentStatusCommand request, CancellationToken cancellationToken)
    {
        ValidateUser();

        var enrollment = await enrollmentRepository.GetByIdAsync(request.EnrollmentId)
                          ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        EnsureUserIsAuthorized(enrollment);

        if (!StatusMap.TryGetValue(request.Status, out var newStatus))
        {
            return Result.Failure("Invalid status value.");
        }

        enrollment.SetStatus(newStatus);

        if (newStatus == EnrollmentStatus.Completed)
        {
            enrollment.MarkAsCompleted();
        }

        await enrollmentRepository.UpdateAsync(enrollment);

        return Result.Success();
    }

    private void ValidateUser()
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }
    }

    private void EnsureUserIsAuthorized(Enrollment enrollment)
    {
        var isOwner = enrollment.StudentId == currentUserService.UserId;
        var isAuthorized = isOwner || currentUserService.IsInRole("Instructor") || currentUserService.IsInRole("Admin");

        if (!isAuthorized)
        {
            throw new ForbiddenAccessException();
        }
    }
}