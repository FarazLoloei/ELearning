using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

namespace ELearning.Application.Common.Exceptions;

public static class PaginationExtensions
{
    public static PaginationParameters ToPaginationParameters<T>(this T paginatable) where T : IPaginatable
    {
        return new PaginationParameters(paginatable.PageNumber, paginatable.PageSize);
    }
}