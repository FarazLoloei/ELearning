using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Submissions.Handlers;

/// <summary>
/// Handler for GetSubmissionDetailQuery
/// </summary>
public class GetSubmissionDetailQueryHandler(
        IAssignmentReadRepository assignmentRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetSubmissionDetailQuery, Result<SubmissionDetailDto>>
{
    public async Task<Result<SubmissionDetailDto>> Handle(GetSubmissionDetailQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
            throw new ForbiddenAccessException();

        var enrollment = await enrollmentRepository.GetBySubmissionIdAsync(request.SubmissionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Submission), request.SubmissionId);
        var submission = enrollment.Submissions.FirstOrDefault(s => s.Id == request.SubmissionId)
            ?? throw new NotFoundException(nameof(Submission), request.SubmissionId);

        await VerifyPermission(enrollment.StudentId, submission.AssignmentId, cancellationToken);

        var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken)
            ?? throw new NotFoundException("Assignment", submission.AssignmentId);
        var student = await userRepository.GetByIdForUpdateAsync(enrollment.StudentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), enrollment.StudentId);

        var grader = submission.GradedById.HasValue
            ? await userRepository.GetByIdForUpdateAsync(submission.GradedById.Value, cancellationToken)
            : null;

        var submissionDto = new SubmissionDetailDto(
            submission.Id,
            submission.AssignmentId,
            assignment.Title,
            submission.SubmittedDate,
            submission.IsGraded,
            submission.Score,
            assignment.MaxPoints,
            enrollment.StudentId,
            student.FullName,
            submission.Content,
            submission.FileUrl,
            submission.Feedback,
            submission.GradedById,
            grader?.FullName ?? string.Empty,
            submission.GradedDate);

        return Result.Success(submissionDto);
    }

    private async Task VerifyPermission(Guid studentId, Guid assignmentId, CancellationToken cancellationToken)
    {
        // Check if current user is the student who submitted, the instructor, or an admin
        var currentUserId = currentUserService.UserId!.Value;
        var isStudent = currentUserId == studentId;
        var isInstructor = false;

        if (!isStudent)
        {
            // Check if user is instructor of course that contains this assignment
            var module = await assignmentRepository.GetModuleForAssignmentAsync(assignmentId, cancellationToken)
                ?? throw new NotFoundException("Module", assignmentId);
            var course = await courseRepository.GetByIdForUpdateAsync(module.CourseId, cancellationToken)
                ?? throw new NotFoundException("Course", module.CourseId);
            isInstructor = course.InstructorId == currentUserId;
        }

        var isAdmin = currentUserService.IsInRole("Admin");

        if (!isStudent && !isInstructor && !isAdmin)
            throw new ForbiddenAccessException();
    }
}