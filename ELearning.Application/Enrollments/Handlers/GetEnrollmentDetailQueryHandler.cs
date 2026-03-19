// <copyright file="GetEnrollmentDetailQueryHandler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Security;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Enrollments.Dtos;
using ELearning.Application.Enrollments.Queries;
using ELearning.Application.Submissions.Dtos;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using MediatR;

public class GetEnrollmentDetailQueryHandler(
        IEnrollmentReadRepository enrollmentReadRepository,
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetEnrollmentDetailQuery, Result<EnrollmentDetailDto>>
{
    public async Task<Result<EnrollmentDetailDto>> Handle(GetEnrollmentDetailQuery request, CancellationToken cancellationToken)
    {
        CurrentUserAuthorizationGuard.EnsureAuthenticated(currentUserService);

        var enrollment = await enrollmentReadRepository.GetByIdAsync(request.EnrollmentId, cancellationToken)
            ?? throw new NotFoundException("Enrollment", request.EnrollmentId);

        await CurrentUserAuthorizationGuard.EnsureEnrollmentReadAccessAsync(
            currentUserService,
            enrollment.StudentId,
            enrollment.CourseId,
            courseRepository,
            cancellationToken);

        var dto = new EnrollmentDetailDto(
            enrollment.Id,
            enrollment.StudentId,
            enrollment.StudentName,
            enrollment.CourseId,
            enrollment.CourseTitle,
            enrollment.Status,
            enrollment.EnrollmentDate,
            enrollment.CompletedDate,
            enrollment.CompletionPercentage,
            enrollment.LessonProgress
                .Select(p => new LessonProgressDto(
                    p.LessonId,
                    p.LessonTitle,
                    p.Status,
                    p.CompletedDate,
                    p.TimeSpentSeconds))
                .ToList(),
            enrollment.Submissions
                .Select(s => new SubmissionDto(
                    s.Id,
                    s.AssignmentId,
                    s.AssignmentTitle,
                    s.SubmittedDate,
                    s.IsGraded,
                    s.Score,
                    s.MaxPoints))
                .ToList(),
            enrollment.CourseRating,
            enrollment.Review);

        return Result.Success(dto);
    }
}
