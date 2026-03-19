// <copyright file="PaginationExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Application.Common.Exceptions;

using ELearning.SharedKernel.Abstractions;
using ELearning.SharedKernel.Models;

public static class PaginationExtensions
{
    public static PaginationParameters ToPaginationParameters<T>(this T paginatable)
        where T : IPaginatable
    {
        return new PaginationParameters(paginatable.PageNumber, paginatable.PageSize);
    }
}