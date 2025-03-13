namespace ELearning.Application.Submissions.Dtos;

// Detailed DTO for submission
public readonly record struct SubmissionDetailDto(
    Guid Id,
    Guid AssignmentId,
    string AssignmentTitle,
    DateTime SubmittedDate,
    bool IsGraded,
    int? Score,
    int MaxPoints,
    Guid StudentId,
    string StudentName,
    string Content,
    string FileUrl,
    string Feedback,
    Guid? GradedById,
    string GradedByName,
    DateTime? GradedDate
);
