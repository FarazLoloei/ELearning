// <copyright file="ICertificateRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Domain.Entities.CertificateAggregate.Abstractions.Repositories;

using ELearning.SharedKernel.Abstractions;

public interface ICertificateRepository : IEntityFrameworkRepository<Certificate>
{
    Task<Certificate?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<Certificate?> GetByCodeAsync(string certificateCode, CancellationToken cancellationToken = default);
}
