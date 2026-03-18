namespace ELearning.Application.Enrollments.ReadModels;

public sealed record LessonProgressReadModel(
    Guid LessonId,
    string LessonTitle,
    string Status,
    DateTime? CompletedDate,
    int TimeSpentSeconds);