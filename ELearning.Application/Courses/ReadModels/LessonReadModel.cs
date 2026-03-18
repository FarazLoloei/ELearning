namespace ELearning.Application.Courses.ReadModels;

public sealed record LessonReadModel(
    Guid Id,
    string Title,
    Guid ModuleId,
    int Order);