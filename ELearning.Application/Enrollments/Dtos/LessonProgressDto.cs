using ELearning.SharedKernel.Abstractions;

namespace ELearning.Application.Enrollments.Dtos;

// DTO for lesson progress
public readonly record struct LessonProgressDto(
    Guid LessonId,
    string LessonTitle,
    string Status,
    DateTime? CompletedDate,
    int TimeSpentSeconds
) : IDto;