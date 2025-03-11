using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Submissions.Handlers;

public class GradeSubmissionCommandHandler(
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
        ICurrentUserService currentUserService,
        IEmailService emailService)
    : IRequestHandler<GradeSubmissionCommand, Result>
{
    public async Task<Result> Handle(GradeSubmissionCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }

        var instructorId = currentUserService.UserId.Value;
        var submission = await submissionRepository.GetByIdAsync(request.SubmissionId);

        if (submission == null)
        {
            throw new NotFoundException(nameof(Submission), request.SubmissionId);
        }

        if (submission.IsGraded)
        {
            return Result.Failure("Submission is already graded.");
        }

        // Get the assignment to check max points
        var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId);
        if (assignment == null)
        {
            throw new NotFoundException(nameof(Assignment), submission.AssignmentId);
        }

        if (request.Score > assignment.MaxPoints)
        {
            return Result.Failure($"Score cannot exceed maximum points ({assignment.MaxPoints}).");
        }

        // Grade the submission
        submission.Grade(request.Score, request.Feedback, instructorId);

        // Save to repository
        await submissionRepository.UpdateAsync(submission);

        // Notify student
        // In a real application, you'd get the student email and name
        // await emailService.SendAssignmentGradedAsync(
        //     studentEmail,
        //     studentName,
        //     assignment.Title,
        //     request.Score);

        return Result.Success();
    }
}