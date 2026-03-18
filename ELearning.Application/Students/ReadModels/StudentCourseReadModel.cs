namespace ELearning.Application.Students.ReadModels;

public sealed record StudentCourseReadModel(
    Guid Id,
    string Title,
    int CategoryId,
    int StatusId,
    DateTime PublishedDate);