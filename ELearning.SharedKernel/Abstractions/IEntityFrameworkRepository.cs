// <copyright file="IEntityFrameworkRepository.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

public interface IEntityFrameworkRepository<T>
    where T : BaseEntity
{
    Task<T?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
