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
        IEnrollmentRepository enrollmentRepository,
        IAssignmentRepository assignmentRepository,
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
            return Result.Failure("Submission is already graded.");

        // Get the assignment to check max points
        var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Assignment), submission.AssignmentId);

        if (request.Score > assignment.MaxPoints)
            return Result.Failure($"Score cannot exceed maximum points ({assignment.MaxPoints}).");

        // Grade the submission
        enrollment.GradeSubmission(request.SubmissionId, request.Score, request.Feedback, instructorId);

        // Save to repository
        await enrollmentRepository.UpdateAsync(enrollment, cancellationToken);

        return Result.Success();
    }
}
