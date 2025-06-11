namespace ELearning.SharedKernel.Abstractions;

public interface IPaginatable
{
    int PageNumber { get; }

    int PageSize { get; }
}