// <copyright file="CertificateIssuanceCoordinator.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Services;

using ELearning.Application.Common.Interfaces;
using ELearning.Domain.Entities.CertificateAggregate;
using ELearning.Domain.Entities.CertificateAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.EnrollmentAggregate.Enums;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;

public sealed class CertificateIssuanceCoordinator(
        ICertificateRepository certificateRepository,
        IUserRepository userRepository,
        IEmailService emailService)
{
    public async Task<Certificate?> TryIssueForCompletedEnrollmentAsync(
        Enrollment enrollment,
        Course course,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(enrollment);
        ArgumentNullException.ThrowIfNull(course);

        if (enrollment.CourseId != course.Id)
        {
            throw new InvalidOperationException("Certificate issuance requires the enrollment and course to match.");
        }

        if (enrollment.Status != EnrollmentStatus.Completed)
        {
            return null;
        }

        var existingCertificate = await certificateRepository.GetByEnrollmentIdAsync(enrollment.Id, cancellationToken);
        if (existingCertificate is not null)
        {
            return existingCertificate;
        }

        var certificate = Certificate.Issue(enrollment.Id, enrollment.StudentId, course.Id);
        await certificateRepository.AddAsync(certificate, cancellationToken);

        var student = await userRepository.GetByIdForUpdateAsync(enrollment.StudentId, cancellationToken)
            ?? throw new InvalidOperationException("Completed enrollment is associated with a missing student.");

        await emailService.SendCertificateIssuedAsync(
            student.Email.Value,
            student.FullName,
            course.Title,
            certificate.CertificateCode);

        return certificate;
    }
}
