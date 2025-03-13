namespace ELearning.SharedKernel.Abstractions;

public interface IReadRepository<T, TKey>
{
    Task<T> GetByIdAsync(TKey id);

    Task<PaginatedList<T>> ListAsync(int pageNumber, int pageSize);
}