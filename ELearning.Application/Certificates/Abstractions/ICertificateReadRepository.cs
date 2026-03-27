// <copyright file="ICertificateReadRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Application.Certificates.Abstractions;

using ELearning.Application.Certificates.ReadModels;

public interface ICertificateReadRepository
{
    Task<CertificateReadModel?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default);

    Task<CertificateReadModel?> GetByCodeAsync(string certificateCode, CancellationToken cancellationToken = default);
}
