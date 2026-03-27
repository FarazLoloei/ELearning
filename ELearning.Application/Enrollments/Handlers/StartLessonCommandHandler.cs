// <copyright file="StartLessonCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using MediatR;

public sealed class StartLessonCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<StartLessonCommand, Result>
{
    public async Task<Result> Handle(StartLessonCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var enrollment = await enrollmentRepository.GetByIdForUpdateAsync(request.EnrollmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        if (enrollment.StudentId != currentUserService.UserId.Value)
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(enrollment.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), enrollment.CourseId);

        if (!course.ContainsLesson(request.LessonId))
        {
            return Result.Failure("The lesson does not belong to the enrolled course.");
        }

        try
        {
            course.EnsureAvailableForLearning();
            enrollment.StartLesson(request.LessonId);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Result.Success();
    }
}
