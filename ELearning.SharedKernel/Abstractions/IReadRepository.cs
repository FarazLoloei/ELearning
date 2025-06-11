using ELearning.SharedKernel.Models;

namespace ELearning.SharedKernel.Abstractions;

public interface IReadRepository<T, TKey>
{
    Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    Task<PaginatedList<T>> ListAsync(PaginationParameters pagination, CancellationToken cancellationToken = default);
}