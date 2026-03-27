// <copyright file="CompleteLessonCommandHandler.cs" company="FarazLoloei">
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

public sealed class CompleteLessonCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<CompleteLessonCommand, Result>
{
    public async Task<Result> Handle(CompleteLessonCommand request, CancellationToken cancellationToken)
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
            enrollment.CompleteLesson(
                request.LessonId,
                course.GetTotalLessonCount(),
                course.GetRequiredAssessmentIds());
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Result.Success();
    }
}
