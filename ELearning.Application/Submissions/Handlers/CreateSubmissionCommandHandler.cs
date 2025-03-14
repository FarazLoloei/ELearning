using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Services;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Exceptions;
using MediatR;

namespace ELearning.Application.Submissions.Handlers;

public class CreateSubmissionCommandHandler(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService,
        IAssignmentService assignmentService) : IRequestHandler<CreateSubmissionCommand, Result>
{
    public async Task<Result> Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }

        // Ensure the assignment exists
        var assignment = await assignmentRepository.GetByIdAsync(request.AssignmentId);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), request.AssignmentId);
        }

        var studentId = currentUserService.UserId.Value;

        // Check if the student can submit to this assignment (this may involve checking if they're enrolled in the course)
        var canSubmit = await assignmentService.CanSubmitAssignmentAsync(studentId, request.AssignmentId);
        if (!canSubmit)
        {
            return Result.Failure("You are not enrolled in the course or the assignment is not available.");
        }

        // Check if the student has already submitted this assignment
        var hasSubmitted = await assignmentService.HasStudentSubmittedAsync(studentId, request.AssignmentId);
        if (hasSubmitted)
        {
            return Result.Failure("You have already submitted this assignment.");
        }

        // Get the enrollment ID for this student and course
        var module = await assignmentRepository.GetModuleForAssignmentAsync(request.AssignmentId);
        if (module == null)
        {
            throw new NotFoundException("Module for assignment", request.AssignmentId);
        }

        var enrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, module.CourseId);
        if (enrollment == null)
        {
            throw new StudentNotEnrolledException(studentId, module.CourseId);
        }

        // Check if the assignment is overdue
        var isOverdue = await assignmentService.IsAssignmentOverdueAsync(request.AssignmentId, DateTime.UtcNow);
        if (isOverdue)
        {
            // This could either return a failure or allow late submissions with a flag
            // For now, we'll allow the submission but could add a "IsLate" flag to the Submission entity
            // return Result.Failure<Guid>("The deadline for this assignment has passed.");
        }

        // Create and store the submission
        var submission = new Submission(
            enrollment.Id,
            request.AssignmentId,
            request.Content,
            request.FileUrl);

        await submissionRepository.AddAsync(submission);

        // Add the submission to the enrollment
        enrollment.AddSubmission(submission);
        await enrollmentRepository.UpdateAsync(enrollment);

        return Result.Success();
    }
}