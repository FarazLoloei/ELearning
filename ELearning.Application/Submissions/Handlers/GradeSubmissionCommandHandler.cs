// <copyright file="GradeSubmissionCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Submissions.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public class GradeSubmissionCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        IAssignmentReadRepository assignmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        ICurrentUserService currentUserService)
    : IRequestHandler<GradeSubmissionCommand, Result>
{
    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var instructorId = currentUserService.UserId.Value;
        var enrollment = await enrollmentRepository.GetBySubmissionIdAsync(request.SubmissionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Submission), request.SubmissionId);
        var submission = enrollment.Submissions.FirstOrDefault(s => s.Id == request.SubmissionId)
            ?? throw new NotFoundException(nameof(Submission), request.SubmissionId);

        if (submission.IsGraded)
        {
            return Result.Failure("Submission is already graded.");
        }

        // Get the assignment to check max points
        var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Assignment), submission.AssignmentId);

        var module = await assignmentRepository.GetModuleForAssignmentAsync(submission.AssignmentId, cancellationToken)
            ?? throw new NotFoundException("Module for assignment", submission.AssignmentId);

        var course = await courseRepository.GetByIdForUpdateAsync(module.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), module.CourseId);

        if (!currentUserService.IsInRole("Admin") && !course.IsOwnedBy(instructorId))
        {
            throw new ForbiddenAccessException();
        }

        try
        {
            assignment.EnsureValidScore(request.Score);
            enrollment.GradeSubmission(
                request.SubmissionId,
                request.Score,
                assignment.MaxPoints,
                request.Feedback,
                instructorId);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentOutOfRangeException)
        {
            return Result.Failure(ex.Message);
        }

        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        var student = await userRepository.GetByIdForUpdateAsync(enrollment.StudentId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), enrollment.StudentId);

        await emailService.SendAssignmentGradedAsync(
            student.Email.Value,
            student.FullName,
            assignment.Title,
            request.Score);

        return Result.Success();
    }
}
