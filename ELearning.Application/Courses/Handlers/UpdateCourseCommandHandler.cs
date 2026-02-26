using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

/// <summary>
/// Handler for UpdateCourseCommand
/// </summary>
public class UpdateCourseCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<UpdateCourseCommand, Result>
{
    public async Task<Result> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            throw new ForbiddenAccessException();

        var course = await courseRepository.GetByIdAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        // Check if the current user is the instructor of this course or an admin
        var isInstructor = course.InstructorId == currentUserService.UserId;

        if (!isInstructor && !currentUserService.IsInRole("Admin"))
            throw new ForbiddenAccessException();

        // Get category and level from enumeration values
        var category = CourseCategory.GetAll<CourseCategory>()
            .FirstOrDefault(c => c.Id == request.CategoryId);

        var level = CourseLevel.GetAll<CourseLevel>()
            .FirstOrDefault(l => l.Id == request.LevelId);

        if (category is null || level is null)
            return Result.Failure($"Invalid category or level. Category: {(category?.Name ?? "null")}, Level: {(level?.Name ?? "null")}");

        // Create duration value object
        //var duration = Duration.Create(request.DurationHours, request.DurationMinutes);

        // Update course details
        course.UpdateDetails(request.Title, request.Description, category, level);
        course.UpdatePrice(request.Price);

        // Toggle featured status if needed
        if (course.IsFeatured != request.IsFeatured)
            course.ToggleFeatured();

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success();
    }
}
