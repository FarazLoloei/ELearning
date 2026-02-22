using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public sealed record LessonDto(
    Guid Id,
    string Title,
    string Content,
    string Type,
    string VideoUrl,
    string Duration,
    int Order
) : IDto;
