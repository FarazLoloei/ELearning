using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Common.Resilience;
using ELearning.Application.Submissions.Abstractions.ReadModels;
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
        ISubmissionReadService submissionReadService,
        ISubmissionRepository submissionRepository,
        IAssignmentRepository assignmentRepository,
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

        try
        {
            // Try Dapr read service first
            var submissionDto = await submissionReadService.GetByIdAsync(request.SubmissionId, cancellationToken);

            // Verify permission
            await VerifyPermission(submissionDto.StudentId, submissionDto.AssignmentId, cancellationToken);

            return Result.Success(submissionDto);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // Fall back to repository
            var submission = await submissionRepository.GetByIdAsync(request.SubmissionId, cancellationToken)
                ?? throw new NotFoundException(nameof(Submission), request.SubmissionId);

            // Get enrollment for this submission to find student ID
            var enrollment = await enrollmentRepository.GetByIdAsync(submission.EnrollmentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Enrollment), submission.EnrollmentId);

            // Verify permission
            await VerifyPermission(enrollment.StudentId, submission.AssignmentId, cancellationToken);

            // Get assignment
            var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId, cancellationToken)
                ?? throw new NotFoundException("Assignment", submission.AssignmentId);

            // Get student and grader (if applicable)
            var student = await userRepository.GetByIdAsync(enrollment.StudentId, cancellationToken)
                ?? throw new NotFoundException(nameof(Student), enrollment.StudentId);

            var grader = submission.GradedById.HasValue
                ? await userRepository.GetByIdAsync(submission.GradedById.Value, cancellationToken)
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
            var course = await courseRepository.GetByIdAsync(module.CourseId, cancellationToken)
                ?? throw new NotFoundException("Course", module.CourseId);
            isInstructor = course.InstructorId == currentUserId;
        }

        var isAdmin = currentUserService.IsInRole("Admin");

        if (!isStudent && !isInstructor && !isAdmin)
            throw new ForbiddenAccessException();
    }
}