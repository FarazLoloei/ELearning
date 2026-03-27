// <copyright file="CertificateIssuanceCoordinatorTests.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Tests.Certificates;

using ELearning.Application.Certificates.Services;
using ELearning.Application.Common.Interfaces;
using ELearning.Domain.Entities.CertificateAggregate;
using ELearning.Domain.Entities.CertificateAggregate.Abstractions.Repositories;
using ELearning.Domain.Entities.CourseAggregate;
using ELearning.Domain.Entities.CourseAggregate.Enums;
using ELearning.Domain.Entities.EnrollmentAggregate;
using ELearning.Domain.Entities.UserAggregate;
using ELearning.Domain.Entities.UserAggregate.Abstractions.Repositories;
using ELearning.Domain.ValueObjects;
using FluentAssertions;

public sealed class CertificateIssuanceCoordinatorTests
{
    [Fact]
    public async Task TryIssueForCompletedEnrollmentAsync_WhenEnrollmentIsNotCompleted_ReturnsNull()
    {
        var student = CreateStudent();
        var coordinator = CreateCoordinator(student, out _, out var emailService);
        var course = CreateCourse();
        var enrollment = new Enrollment(student.Id, course.Id);

        var certificate = await coordinator.TryIssueForCompletedEnrollmentAsync(
            enrollment,
            course,
            TestContext.Current.CancellationToken);

        certificate.Should().BeNull();
        emailService.CertificateEmailsSent.Should().Be(0);
    }

    [Fact]
    public async Task TryIssueForCompletedEnrollmentAsync_WhenCalledTwice_IssuesOnlyOneCertificate()
    {
        var student = CreateStudent();
        var coordinator = CreateCoordinator(student, out var repository, out var emailService);
        var course = CreateCourse();
        var enrollment = new Enrollment(student.Id, course.Id);
        enrollment.CompleteLesson(Guid.NewGuid(), totalLessonsInCourse: 1, requiredAssignmentIds: []);

        var firstCertificate = await coordinator.TryIssueForCompletedEnrollmentAsync(
            enrollment,
            course,
            TestContext.Current.CancellationToken);

        var secondCertificate = await coordinator.TryIssueForCompletedEnrollmentAsync(
            enrollment,
            course,
            TestContext.Current.CancellationToken);

        firstCertificate.Should().NotBeNull();
        secondCertificate.Should().NotBeNull();
        secondCertificate!.Id.Should().Be(firstCertificate!.Id);
        repository.StoredCertificates.Should().ContainSingle();
        emailService.CertificateEmailsSent.Should().Be(1);
    }

    private static CertificateIssuanceCoordinator CreateCoordinator(
        Student student,
        out InMemoryCertificateRepository certificateRepository,
        out FakeEmailService emailService)
    {
        certificateRepository = new InMemoryCertificateRepository();
        emailService = new FakeEmailService();
        var userRepository = new InMemoryUserRepository(student);

        return new CertificateIssuanceCoordinator(certificateRepository, userRepository, emailService);
    }

    private static Student CreateStudent() =>
        new(
            "Test",
            "Student",
            Email.Create("student@example.com"),
            "hashed-password");

    private static Course CreateCourse() =>
        new(
            "Distributed Systems",
            "Build a production-inspired backend.",
            Guid.NewGuid(),
            CourseCategory.Programming,
            CourseLevel.Advanced,
            Duration.Create(5, 0),
            149);

    private sealed class InMemoryCertificateRepository : ICertificateRepository
    {
        private readonly List<Certificate> certificates = [];

        public IReadOnlyList<Certificate> StoredCertificates => this.certificates;

        public Task<Certificate?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(this.certificates.SingleOrDefault(certificate => certificate.Id == id));

        public Task<Certificate?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) =>
            Task.FromResult(this.certificates.SingleOrDefault(certificate => certificate.EnrollmentId == enrollmentId));

        public Task<Certificate?> GetByCodeAsync(string certificateCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(this.certificates.SingleOrDefault(certificate => certificate.CertificateCode == certificateCode));

        public Task AddAsync(Certificate entity, CancellationToken cancellationToken = default)
        {
            this.certificates.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Certificate entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(Certificate entity, CancellationToken cancellationToken = default)
        {
            this.certificates.Remove(entity);
            return Task.CompletedTask;
        }
    }

    private sealed class InMemoryUserRepository(Student student) : IUserRepository
    {
        public Task<User?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(student.Id == id ? student : null);

        public Task AddAsync(User entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task DeleteAsync(User entity, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult<User?>(string.Equals(student.Email.Value, email, StringComparison.OrdinalIgnoreCase) ? student : null);

        public Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(!string.Equals(student.Email.Value, email, StringComparison.OrdinalIgnoreCase));

        public Task<int> GetUsersCountAsync(CancellationToken cancellationToken = default) => Task.FromResult(1);
    }

    private sealed class FakeEmailService : IEmailService
    {
        public int CertificateEmailsSent { get; private set; }

        public Task SendEmailAsync(string to, string subject, string body, bool isHtml = false) => Task.CompletedTask;

        public Task SendEnrollmentConfirmationAsync(string to, string studentName, string courseName) => Task.CompletedTask;

        public Task SendAssignmentGradedAsync(string to, string studentName, string assignmentName, int score) => Task.CompletedTask;

        public Task SendCourseApprovedAsync(string to, string instructorName, string courseName) => Task.CompletedTask;

        public Task SendCourseRejectedAsync(string to, string instructorName, string courseName, string reason) => Task.CompletedTask;

        public Task SendCertificateIssuedAsync(string to, string studentName, string courseName, string certificateCode)
        {
            this.CertificateEmailsSent++;
            return Task.CompletedTask;
        }
    }
}
