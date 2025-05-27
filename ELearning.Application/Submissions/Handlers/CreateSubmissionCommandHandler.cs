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
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            throw new ForbiddenAccessException();

        var studentId = currentUserService.UserId.Value;

        // Ensure the assignment exists
        var assignment = await assignmentRepository.GetByIdAsync(request.AssignmentId)
            ?? throw new NotFoundException(nameof(Assignment), request.AssignmentId);

        // Verify submission eligibility
        if (!await assignmentService.CanSubmitAssignmentAsync(studentId, request.AssignmentId))
            return Result.Failure("You are not enrolled in the course or the assignment is not available.");

        if (await assignmentService.HasStudentSubmittedAsync(studentId, request.AssignmentId))
            return Result.Failure("You have already submitted this assignment.");

        // Get module and enrollment
        var module = await assignmentRepository.GetModuleForAssignmentAsync(request.AssignmentId)
            ?? throw new NotFoundException("Module for assignment", request.AssignmentId);

        var enrollment = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, module.CourseId)
            ?? throw new StudentNotEnrolledException(studentId, module.CourseId);

        // (Optional) Handle late submissions
        var isOverdue = await assignmentService.IsAssignmentOverdueAsync(request.AssignmentId, DateTime.UtcNow);
        if (isOverdue)
        {
            // This could either return a failure or allow late submissions with a flag
            // For now, we'll allow the submission but could add a "IsLate" flag to the Submission entity
            // return Result.Failure<Guid>("The deadline for this assignment has passed.");
        }

        var submission = new Submission(
            enrollment.Id,
            request.AssignmentId,
            request.Content,
            request.FileUrl);

        await submissionRepository.AddAsync(submission);

        // Link submission to enrollment
        enrollment.AddSubmission(submission);
        await enrollmentRepository.UpdateAsync(enrollment);

        return Result.Success();
    }
}