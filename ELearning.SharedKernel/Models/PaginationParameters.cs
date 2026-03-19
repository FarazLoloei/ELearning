// <copyright file="PaginationParameters.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ELearning.SharedKernel.Models;

/// <summary>
/// Represents common pagination parameters.
/// </summary>
public class PaginationParameters
{
    private int pageNumber = 1;
    private int pageSize = 10;
    private const int MaxPageSize = 100; // Define a maximum page size to prevent abuse

    public int PageNumber
    {
        get => this.pageNumber;
        set => this.pageNumber = (value < 1) ? 1 : value; // Ensure PageNumber is at least 1
    }

    public int PageSize
    {
        get => this.pageSize;
        set => this.pageSize = (value > MaxPageSize) ? MaxPageSize : (value < 1) ? 1 : value; // Ensure PageSize is positive and within max
    }

    public PaginationParameters()
    {
    }

    public PaginationParameters(int pageNumber, int pageSize)
    {
        this.PageNumber = pageNumber;
        this.PageSize = pageSize;
    }

    /// <summary>
    /// Gets the number of items to skip for the current page.
    /// </summary>
    public int SkipCount => (this.PageNumber - 1) * this.PageSize; // Changed to a readonly property
}