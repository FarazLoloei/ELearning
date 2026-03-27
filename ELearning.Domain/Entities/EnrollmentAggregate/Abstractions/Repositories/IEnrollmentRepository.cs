// <copyright file="IEnrollmentRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.EnrollmentAggregate.Abstractions.Repositories;

using ELearning.SharedKernel.Abstractions;

public interface IEnrollmentRepository : IEntityFrameworkRepository<Enrollment>
{
    Task<IReadOnlyList<Enrollment>> GetByStudentIdAsync(Guid studentId, CancellationToken cancellationToken);

    Task<Enrollment?> GetByStudentAndCourseIdAsync(Guid studentId, Guid courseId, CancellationToken cancellationToken);

    Task<Enrollment?> GetBySubmissionIdAsync(Guid submissionId, CancellationToken cancellationToken);

    Task<bool> HasAnyForCourseAsync(Guid courseId, CancellationToken cancellationToken);
}
