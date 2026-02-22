using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Courses.Dtos;

public readonly record struct ModuleDto(
    Guid Id,
    string Title,
    string Description,
    int Order,
    List<LessonDto> Lessons,
    List<AssignmentDto> Assignments
) : IDto;