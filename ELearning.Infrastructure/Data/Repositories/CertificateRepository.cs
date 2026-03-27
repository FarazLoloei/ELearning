// <copyright file="CertificateRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.Data.Repositories;

using ELearning.Domain.Entities.CertificateAggregate;
using ELearning.Domain.Entities.CertificateAggregate.Abstractions.Repositories;
using Microsoft.EntityFrameworkCore;

public sealed class CertificateRepository(ApplicationDbContext context) : ICertificateRepository
{
    public async Task<Certificate?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Certificates.FirstOrDefaultAsync(certificate => certificate.Id == id, cancellationToken);

    public async Task<Certificate?> GetByEnrollmentIdAsync(Guid enrollmentId, CancellationToken cancellationToken = default) =>
        await context.Certificates.FirstOrDefaultAsync(certificate => certificate.EnrollmentId == enrollmentId, cancellationToken);

    public async Task<Certificate?> GetByCodeAsync(string certificateCode, CancellationToken cancellationToken = default) =>
        await context.Certificates.FirstOrDefaultAsync(certificate => certificate.CertificateCode == certificateCode, cancellationToken);

    public async Task AddAsync(Certificate entity, CancellationToken cancellationToken = default) =>
        await context.Certificates.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Certificate entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Certificate entity, CancellationToken cancellationToken = default)
    {
        context.Certificates.Remove(entity);
        return Task.CompletedTask;
    }
}
