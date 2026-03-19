// <copyright file="IReadRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

using ELearning.SharedKernel.Models;

public interface IReadRepository<T, TKey>
{
    Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<PaginatedList<T>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
}