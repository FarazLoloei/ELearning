using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public sealed record ReviewDto(
    Guid Id,
    string StudentName,
    decimal Rating,
    string Comment,
    DateTime CreatedAt
) : IDto;
