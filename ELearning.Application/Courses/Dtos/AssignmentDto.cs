using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public sealed record AssignmentDto(
    Guid Id,
    string Title,
    string Description,
    string Type,
    int MaxPoints,
    DateTime? DueDate
) : IDto;
