namespace ELearning.Application.Courses.Dtos;

public readonly record struct AssignmentDto(
    Guid Id,
    string Title,
    string Description,
    string Type,
    int MaxPoints,
    DateTime? DueDate
);