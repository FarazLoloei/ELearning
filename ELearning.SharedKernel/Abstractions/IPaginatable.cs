// <copyright file="IPaginatable.cs" company="FarazLoloei">
// Copyright (c) FarazLoloei. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

public interface IPaginatable
{
    int PageNumber { get; }

    int PageSize { get; }
}