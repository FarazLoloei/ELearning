using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Submissions.Abstractions.ReadModels;
using ELearning.Application.Submissions.Dtos;
using ELearning.Application.Submissions.Queries;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.SharedKernel;
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
        if (!currentUserService.IsAuthenticated || currentUserService.UserId == null)
        {
            throw new ForbiddenAccessException();
        }

        // Ensure current user is the instructor or an admin
        var currentUserId = currentUserService.UserId.Value;
        var isRequestedInstructor = currentUserId == request.InstructorId;
        var isAdmin = currentUserService.IsInRole("Admin");

        if (!isRequestedInstructor && !isAdmin)
        {
            throw new ForbiddenAccessException();
        }

        try
        {
            // Try Dapr read service first
            var paginatedList = await submissionReadService.GetPendingSubmissionsAsync(
                request.InstructorId,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
        catch (Exception)
        {
            // Fall back to repositories

            // Get all courses by this instructor
            var instructorCourses = await courseRepository.GetByInstructorIdAsync(request.InstructorId);
            var courseIds = instructorCourses.Select(c => c.Id).ToList();

            // Get all assignments in these courses
            var assignmentIds = new List<Guid>();
            foreach (var courseId in courseIds)
            {
                var assignments = await assignmentRepository.GetByCourseIdAsync(courseId);
                assignmentIds.AddRange(assignments.Select(a => a.Id));
            }

            // Get all ungraded submissions for these assignments
            var allSubmissions = new List<Submission>();
            foreach (var assignmentId in assignmentIds)
            {
                var submissions = await submissionRepository.GetByAssignmentIdAsync(assignmentId);
                allSubmissions.AddRange(submissions.Where(s => !s.IsGraded));
            }

            // Order by submission date (oldest first)
            var orderedSubmissions = allSubmissions
                .OrderBy(s => s.SubmittedDate)
                .ToList();

            // Apply pagination
            var totalCount = orderedSubmissions.Count;
            var paginatedSubmissions = orderedSubmissions
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var submissionDtos = new List<SubmissionDto>();
            foreach (var submission in paginatedSubmissions)
            {
                var assignment = await assignmentRepository.GetByIdAsync(submission.AssignmentId);

                var submissionDto = new SubmissionDto
                {
                    Id = submission.Id,
                    AssignmentId = submission.AssignmentId,
                    AssignmentTitle = assignment.Title,
                    SubmittedDate = submission.SubmittedDate,
                    IsGraded = submission.IsGraded,
                    Score = submission.Score,
                    MaxPoints = assignment.MaxPoints
                };

                submissionDtos.Add(submissionDto);
            }

            var paginatedList = new PaginatedList<SubmissionDto>(
                submissionDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);

            return Result.Success(paginatedList);
        }
    }
}