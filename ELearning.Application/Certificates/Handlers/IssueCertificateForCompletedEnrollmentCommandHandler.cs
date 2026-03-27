// <copyright file="IssueCertificateForCompletedEnrollmentCommandHandler.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Handlers;

using ELearning.Application.Certificates.Commands;
using ELearning.Application.Certificates.Dtos;
using ELearning.Application.Certificates.Services;
using ELearning.Application.Common.Exceptions;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Model;
using ELearning.Domain.Entities.CertificateAggregate;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using MediatR;

public sealed class IssueCertificateForCompletedEnrollmentCommandHandler(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        CertificateIssuanceCoordinator certificateIssuanceCoordinator)
    : IRequestHandler<IssueCertificateForCompletedEnrollmentCommand, Result<CertificateDto>>
{
    public async Task<Result<CertificateDto>> Handle(
        IssueCertificateForCompletedEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        if (!currentUserService.IsAuthenticated || currentUserService.UserId is null)
        {
            throw new ForbiddenAccessException();
        }

        var enrollment = await enrollmentRepository.GetByIdForUpdateAsync(request.EnrollmentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Enrollment), request.EnrollmentId);

        var isOwner = enrollment.StudentId == currentUserService.UserId.Value;
        if (!isOwner && !currentUserService.IsInRole("Admin"))
        {
            throw new ForbiddenAccessException();
        }

        var course = await courseRepository.GetByIdForUpdateAsync(enrollment.CourseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Course), enrollment.CourseId);

        var student = await userRepository.GetByIdForUpdateAsync(enrollment.StudentId, cancellationToken)
            ?? throw new NotFoundException(nameof(User), enrollment.StudentId);

        Certificate? certificate;

        try
        {
            certificate = await certificateIssuanceCoordinator.TryIssueForCompletedEnrollmentAsync(
                enrollment,
                course,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<CertificateDto>(ex.Message);
        }

        if (certificate is null)
        {
            return Result.Failure<CertificateDto>("Certificates are available only after successful course completion.");
        }

        return Result.Success(new CertificateDto(
            certificate.Id,
            certificate.CertificateCode,
            certificate.EnrollmentId,
            certificate.StudentId,
            student.FullName,
            certificate.CourseId,
            course.Title,
            certificate.IssuedOnUtc));
    }
}
