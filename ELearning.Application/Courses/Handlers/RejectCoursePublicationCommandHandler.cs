// <copyright file="RejectCoursePublicationCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Courses.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Courses.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public sealed class RejectCoursePublicationCommandHandler(
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        IEmailService emailService,
        ICurrentUserService currentUserService)
    : IRequestHandler<RejectCoursePublicationCommand, Result>
{
    public async Task<Result> Handle(RejectCoursePublicationCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        try
        {
            course.RejectPublication(request.Reason);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            return Result.Failure(ex.Message);
        }

        await courseRepository.UpdateAsync(course, cancellationToken);

        var instructor = await userRepository.GetByIdForUpdateAsync(course.InstructorId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), course.InstructorId);

        await emailService.SendCourseRejectedAsync(
            instructor.Email.Value,
            instructor.FullName,
            course.Title,
            course.RejectionReason ?? request.Reason);

        return Result.Success();
    }
}
