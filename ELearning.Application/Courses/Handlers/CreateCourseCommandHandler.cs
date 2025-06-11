using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.ValueObjects;
using MediatR;

namespace ELearning.Application.Courses.Handlers;

public class CreateCourseCommandHandler(ICourseRepository courseRepository,
        ICurrentUserService currentUserService,
        IInstructorRepository instructorRepository)
    : IRequestHandler<CreateCourseCommand, Result>
{
    public async Task<Result> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
            throw new ForbiddenAccessException();

        var instructorId = currentUserService.UserId.Value;
        var instructor = await instructorRepository.GetByIdAsync(instructorId) ??
            throw new ForbiddenAccessException();

        // Get category and level from enumeration values
        var category = CourseCategory.GetAll<CourseCategory>().FirstOrDefault(c => c.Id == request.CategoryId);
        var level = CourseLevel.GetAll<CourseLevel>().FirstOrDefault(l => l.Id == request.LevelId);

        if (category is null || level is null)
        {
            return Result.Failure($"Invalid category or level. Category: {(category?.Name ?? "null")}, Level: {(level?.Name ?? "null")}");
        }

        // Create duration value object
        var duration = Duration.Create(request.DurationHours, request.DurationMinutes);

        // Create course entity
        var course = new Course(
            request.Title,
            request.Description,
            instructorId,
            category,
            level,
            duration,
            request.Price);

        await courseRepository.AddAsync(course);

        return Result.Success();
    }
}