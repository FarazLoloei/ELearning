namespace ELearning.SharedKernel.Models;

/// <summary>
/// Represents common pagination parameters.
/// </summary>
public class PaginationParameters
{
    public int PageNumber { get; set; } = 1; // Default to page 1

    public int PageSize { get; set; } = 10;  // Default to 10 items

    public PaginationParameters()
    { }

    public PaginationParameters(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}