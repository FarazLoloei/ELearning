// <copyright file="CreateEnrollmentCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Handlers;

using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Application.Enrollments.Commands;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate.Enums;
using MediatR;

public class CreateEnrollmentCommandHandler(
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            IEmailService emailService,
            ICurrentUserService currentUserService)
    : IRequestHandler<CreateEnrollmentCommand, Result>
{
    public async Task<Result> Handle(CreateEnrollmentCommand request, CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var studentId = currentUserService.UserId.Value;
        var student = await userRepository.GetByIdForUpdateAsync(studentId, cancellationToken);
        if (student is null || student.Role.Id != UserRole.Student.Id)
        {
            throw new NotFoundException("Student", studentId);
        }

        var course = await courseRepository.GetByIdForUpdateAsync(request.CourseId, cancellationToken) ??
            throw new NotFoundException(nameof(Course), request.CourseId);

        try
        {
            course.EnsureCanAcceptNewEnrollments();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        // Check if student is already enrolled
        var alreadyEnrolled = await enrollmentRepository.GetByStudentAndCourseIdAsync(studentId, request.CourseId, cancellationToken) is not null;
        if (alreadyEnrolled)
        {
            return Result.Failure("You are already enrolled in this course.");
        }

        var enrollment = new Enrollment(studentId, course.Id);
        await enrollmentRepository.AddAsync(enrollment, cancellationToken);
        await emailService.SendEnrollmentConfirmationAsync(student.Email.Value, student.FullName, course.Title);

        return Result.Success();
    }
}
