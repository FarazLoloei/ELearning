namespace ELearning.Application.Submissions.Dtos;

// DTO for submission
public readonly record struct SubmissionDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    DateTime SubmittedDate,
    bool IsGraded,
    int? Score,
    int MaxPoints
);