namespace ELearning.Application.Enrollments.ReadModels;

public sealed record ProgressReadModel(
    Guid Id,
    Guid EnrollmentId,
    Guid LessonId,
    int StatusId,
    string StatusName,
    DateTime? CompletedDate,
    int TimeSpentSeconds);