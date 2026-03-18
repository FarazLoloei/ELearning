namespace ELearning.Application.Enrollments.ReadModels;

public sealed record EnrollmentDetailReadModel(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    IReadOnlyList<LessonProgressReadModel> LessonProgress,
    IReadOnlyList<EnrollmentSubmissionReadModel> Submissions,
    decimal? CourseRating,
    string? Review);