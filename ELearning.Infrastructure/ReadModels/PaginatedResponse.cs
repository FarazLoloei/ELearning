// <copyright file="PaginatedResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.Infrastructure.ReadModels;

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new List<T>();

    public int TotalCount { get; set; }
}