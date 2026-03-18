namespace ELearning.Application.Enrollments.ReadModels;

public sealed record EnrollmentSubmissionReadModel(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    DateTime SubmittedDate,
    bool IsGraded,
    int? Score,
    int MaxPoints);
