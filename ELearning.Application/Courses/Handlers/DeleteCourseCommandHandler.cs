using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

/// <summary>
/// Handler for DeleteCourseCommand
/// </summary>
public class DeleteCourseCommandHandler(
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<DeleteCourseCommand, Result>
{
    public async Task<Result> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            throw new ForbiddenAccessException();

        var course = await courseRepository.GetByIdAsync(request.CourseId) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        // Check if the current user is the instructor of this course or an admin
        var isInstructor = course.InstructorId == currentUserService.UserId;

        if (!isInstructor && !currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        // Check if there are any enrollments for this course
        var enrollments = await enrollmentRepository.GetByCourseIdAsync(request.CourseId);

        if (enrollments.Any())
            return Result.Failure("Cannot delete a course with active enrollments. Archive it instead.");

        // Delete the course
        await courseRepository.DeleteAsync(course);

        return Result.Success();
    }
}