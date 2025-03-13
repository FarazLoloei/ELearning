namespace ELearning.SharedKernel.Abstractions;

public interface IEntityFrameworkRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> ListAllAsync();

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);
}