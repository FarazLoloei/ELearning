using ELearning.Application.Common.Model;

namespace ELearning.Infrastructure.Dapr.Abstraction;

public interface IReadService<T, TKey>
{
    Task<T> GetByIdAsync(TKey id);

    Task<PaginatedList<T>> ListAsync(int pageNumber, int pageSize);
}