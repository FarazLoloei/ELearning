namespace ELearning.SharedKernel.Models;

/// <summary>
/// Represents common pagination parameters.
/// </summary>
public class PaginationParameters
{
    private int _pageNumber = 1;
    private int _pageSize = 10;
    private const int MaxPageSize = 100; // Define a maximum page size to prevent abuse

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = (value < 1) ? 1 : value; // Ensure PageNumber is at least 1
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value < 1) ? 1 : value; // Ensure PageSize is positive and within max
    }

    public PaginationParameters()
    { }

    public PaginationParameters(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets the number of items to skip for the current page.
    /// </summary>
    public int SkipCount => (PageNumber - 1) * PageSize; // Changed to a readonly property
}