// <copyright file="IPaginatable.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Abstractions;

public interface IPaginatable
{
    int PageNumber { get; }

    int PageSize { get; }
}