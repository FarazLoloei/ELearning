using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public readonly record struct ReviewDto(
    Guid Id,
    string StudentName,
    decimal Rating,
    string Comment,
    DateTime CreatedAt
) : IDto;