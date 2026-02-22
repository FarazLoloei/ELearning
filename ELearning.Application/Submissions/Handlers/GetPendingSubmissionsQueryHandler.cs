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
using ELearning.SharedKernel;
using ELearning.SharedKernel.Models;
using MediatR;

namespace ELearning.Application.Submissions.Handlers;

/// <summary>
/// Handler for GetPendingSubmissionsQuery
/// </summary>
public class GetPendingSubmissionsQueryHandler(
        ISubmissionReadService submissionReadService,
        ICourseRepository courseRepository,
        IAssignmentRepository assignmentRepository,
        ISubmissionRepository submissionRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<GetPendingSubmissionsQuery, Result<PaginatedList<SubmissionDto>>>
{
    public async Task<Result<PaginatedList<SubmissionDto>>> Handle(GetPendingSubmissionsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            throw new ForbiddenAccessException();

        // Ensure current user is the instructor or an admin
        var currentUserId = currentUserService.UserId.Value;

        if (currentUserId != request.InstructorId && !currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        try
        {
            // Try Dapr read service first
            var paginatedList = await submissionReadService.GetPendingSubmissionsAsync(
                request.InstructorId,
                new PaginationParameters(request.PageNumber, request.PageSize));

            return Result.Success(paginatedList);
        }
        catch (Exception ex) when (ReadModelFallbackPolicy.ShouldFallback(ex, cancellationToken))
        {
            // Fall back to repositories

            // Get all courses by this instructor
            var courseIds = (await courseRepository.GetByInstructorIdAsync(request.InstructorId, cancellationToken))
                            .Select(c => c.Id)
                            .ToList();

            // Get all assignments in these courses
            var assignmentIds = new List<Guid>();
            foreach (var courseId in courseIds)
            {
                var assignments = await assignmentRepository.GetByCourseIdAsync(courseId);
                assignmentIds.AddRange(assignments.Select(a => a.Id));
            }

            // Get all ungraded submissions for these assignments
            var ungradedSubmissions = new List<Submission>();
            foreach (var assignmentId in assignmentIds)
            {
                var submissions = await submissionRepository.GetByAssignmentIdAsync(assignmentId);
                ungradedSubmissions.AddRange(submissions.Where(s => !s.IsGraded));
            }

            // Order by submission date (oldest first)
            var orderedSubmissions = ungradedSubmissions
                .OrderBy(s => s.SubmittedDate)
                .ToList();

            // Apply pagination
            var paginatedSubmissions = orderedSubmissions
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var submissionDtos = new List<SubmissionDto>();
            foreach (var submission in paginatedSubmissions)
            {
                var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId);

                submissionDtos.Add(new SubmissionDto(
                    submission.Id,
                    submission.AssignmentId,
                    assignment.Title,
                    submission.SubmittedDate,
                    submission.IsGraded,
                    submission.Score,
                    assignment.MaxPoints));
            }

            var paginatedList = new PaginatedList<SubmissionDto>(
                submissionDtos,
                orderedSubmissions.Count,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
    }
}

