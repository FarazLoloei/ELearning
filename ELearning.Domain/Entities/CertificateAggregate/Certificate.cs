// <copyright file="Certificate.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CertificateAggregate;

using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;

public sealed class Certificate : BaseEntity, IAggregateRoot<Certificate>
{
    public Guid EnrollmentId { get; private set; }

    public Guid StudentId { get; private set; }

    public Guid CourseId { get; private set; }

    public string CertificateCode { get; private set; } = string.Empty;

    public DateTime IssuedOnUtc { get; private set; }

    private Certificate()
    {
    }

    private Certificate(Guid enrollmentId, Guid studentId, Guid courseId, string certificateCode)
    {
        if (enrollmentId == Guid.Empty)
        {
            throw new ArgumentException("Enrollment ID is required.", nameof(enrollmentId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student ID is required.", nameof(studentId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course ID is required.", nameof(courseId));
        }

        if (string.IsNullOrWhiteSpace(certificateCode))
        {
            throw new ArgumentException("Certificate code is required.", nameof(certificateCode));
        }

        this.EnrollmentId = enrollmentId;
        this.StudentId = studentId;
        this.CourseId = courseId;
        this.CertificateCode = certificateCode.Trim();
        this.IssuedOnUtc = DateTime.UtcNow;
    }

    public static Certificate Issue(Guid enrollmentId, Guid studentId, Guid courseId) =>
        new(
            enrollmentId,
            studentId,
            courseId,
            $"CERT-{Guid.NewGuid():N}".ToUpperInvariant());
}
