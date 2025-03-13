namespace ELearning.Application.Courses.Dtos;

public readonly record struct LessonDto(
    Guid Id,
    string Title,
    string Content,
    string Type,
    string VideoUrl,
    string Duration,
    int Order
);