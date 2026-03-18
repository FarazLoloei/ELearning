namespace ELearning.Application.Courses.Abstractions;

public sealed record ModuleReadModel(
    Guid Id,
    string Title,
    string Description,
    int Order,
    Guid CourseId);