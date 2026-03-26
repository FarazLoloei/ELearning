// <copyright file="GetStudentProgressQueryHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Students.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Security;
using ELearning.Application.Enrollments.Abstractions;
using ELearning.Application.Students.Abstractions;
using ELearning.Application.Students.Dtos;
using ELearning.Application.Students.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using MediatR;

public class GetStudentProgressQueryHandler(
        IStudentReadRepository studentReadRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IProgressReadRepository progressReadRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetStudentProgressQuery, Result<StudentProgressDto>>
{
    public async Task<Result<StudentProgressDto>> Handle(GetStudentProgressQuery request, CancellationToken cancellationToken)
    {
        CurrentUserAuthorizationGuard.EnsureStudentSelfOrAdmin(currentUserService, request.StudentId);

        var student = await studentReadRepository.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException("Student", request.StudentId);

        var enrollments = await enrollmentRepository.GetByStudentIdAsync(request.StudentId, cancellationToken);

        var enrollmentProgressDtos = new List<EnrollmentProgressDto>();
        foreach (var enrollment in enrollments)
        {
            var course = await courseRepository.GetByIdForUpdateAsync(enrollment.CourseId, cancellationToken)
                ?? throw new NotFoundException("Course", enrollment.CourseId);

            var totalLessons = course.Modules.Sum(m => m.Lessons.Count);
            var completedLessons = enrollment.ProgressRecords.Count(p => p.Status == ProgressStatus.Completed);
            var totalAssignments = course.Modules.Sum(m => m.Assignments.Count);
            var completedAssignments = enrollment.Submissions.Count;
            var completionPercentage = await progressReadRepository.GetCourseProgressPercentageAsync(enrollment.Id, cancellationToken);

            enrollmentProgressDtos.Add(new EnrollmentProgressDto(
                enrollment.Id,
                enrollment.CourseId,
                course.Title,
                enrollment.Status.Name,
                enrollment.CreatedAt(),
                enrollment.CompletedDateUTC,
                completionPercentage,
                completedLessons,
                totalLessons,
                completedAssignments,
                totalAssignments));
        }

        var completedCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Completed);
        var inProgressCourses = enrollments.Count(e => e.Status == EnrollmentStatus.Active || e.Status == EnrollmentStatus.Paused);

        var progressDto = new StudentProgressDto(
            student.Id,
            student.FullName,
            completedCourses,
            inProgressCourses,
            enrollmentProgressDtos);

        return Result.Success(progressDto);
    }
}
