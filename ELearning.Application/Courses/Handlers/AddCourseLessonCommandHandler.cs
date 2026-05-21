// <copyright file="AddCourseLessonCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.CourseAggregate.Exceptions;
using ELearning.Domain.ValueObjects;
using MediatR;

public sealed class AddCourseLessonCommandHandler(
        ICourseRepository courseRepository,
        ICurrentUserService currentUserService)
    : IRequestHandler<AddCourseLessonCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(AddCourseLessonCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        if (!course.IsOwnedBy(currentUserService.UserId.Value))
        {
            throw new ForbiddenAccessException();
        }

        var lessonType = LessonType.GetAll<LessonType>()
            .FirstOrDefault(type => type.Id == request.TypeId);

        if (lessonType is null)
        {
            return Result.Failure<Guid>($"Invalid lesson type: {request.TypeId}");
        }

        Lesson lesson;
        try
        {
            var duration = Duration.Create(request.DurationHours, request.DurationMinutes);
            lesson = new Lesson(
                request.Title,
                request.Content,
                lessonType,
                request.Order,
                request.ModuleId,
                duration,
                request.VideoUrl);

            course.AddLessonToModule(request.ModuleId, lesson);
        }
        catch (ModuleNotFoundException)
        {
            throw new NotFoundException(nameof(Module), request.ModuleId);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or ArgumentOutOfRangeException)
        {
            return Result.Failure<Guid>(ex.Message);
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        return Result.Success(lesson.Id);
    }
}
