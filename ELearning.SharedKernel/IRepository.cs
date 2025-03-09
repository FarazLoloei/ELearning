﻿namespace ELearning.SharedKernel;

public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> ListAllAsync();

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}