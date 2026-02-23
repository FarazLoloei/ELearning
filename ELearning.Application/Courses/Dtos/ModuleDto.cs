using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public sealed record ModuleDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    IReadOnlyList<LessonDto> Lessons,
    IReadOnlyList<AssignmentDto> Assignments
) : IDto;
