// <copyright file="CreateSubmissionCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Handlers;

using ELearning.Application.Certificates.Services;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Exceptions;
using MediatR;

public class CreateSubmissionCommandHandler(
        IAssignmentReadRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        CertificateIssuanceCoordinator certificateIssuanceCoordinator,
        ICurrentUserService currentUserService) : IRequestHandler<CreateSubmissionCommand, Result>
{
    public async Task<Result> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var studentId = currentUserService.UserId.Value;

        // Ensure the assignment exists
        var assignment = await assignmentRepository.GetByIdAsync(request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Assignment), request.AssignmentId);

        var module = await assignmentRepository.GetModuleForAssignmentAsync(request.AssignmentId, cancellationToken)
            ?? throw new NotFoundException("Module for assignment", request.AssignmentId);

        var course = await courseRepository.GetByIdForUpdateAsync(module.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), module.CourseId);

        if (!course.ContainsAssignment(request.AssignmentId))
        {
            return Result.Failure("The assessment does not belong to the enrolled course.");
        }

        var enrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, module.CourseId, cancellationToken)
            ?? throw new StudentNotEnrolledException(studentId, module.CourseId);

        try
        {
            course.EnsureAvailableForLearning();
            assignment.EnsureCanAcceptSubmissionAt(DateTime.UtcNow);
            enrollment.SubmitAssignment(
                request.AssignmentId,
                request.Content,
                request.FileUrl,
                course.GetTotalLessonCount(),
                course.GetRequiredAssessmentIds());
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentOutOfRangeException)
        {
            return Result.Failure(ex.Message);
        }

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);
        await certificateIssuanceCoordinator.TryIssueForCompletedEnrollmentAsync(enrollment, course, cancellationToken);

        return Result.Success();
    }
}
