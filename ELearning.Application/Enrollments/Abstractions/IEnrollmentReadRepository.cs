// <copyright file="IEnrollmentReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Enrollments.Abstractions;

using ELearning.Application.Enrollments.ReadModels;
using ELearning.SharedKernel;
using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

public interface IEnrollmentReadRepository : IReadRepository<EnrollmentDetailReadModel, Guid>
{
    new Task<EnrollmentDetailReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PaginatedList<EnrollmentSummaryReadModel>> GetStudentEnrollmentsAsync(
        Guid studentId,
        PaginationParameters pagination,
        CancellationToken cancellationToken = default);
}