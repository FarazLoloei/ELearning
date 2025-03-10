namespace ELearning.Application.Enrollments.Dtos;

// Detailed DTO for enrollment
public readonly record struct EnrollmentDetailDto(
    Guid Id,
    Guid StudentId,
    string StudentName,
    Guid CourseId,
    string CourseTitle,
    string Status,
    DateTime EnrollmentDate,
    DateTime? CompletedDate,
    double CompletionPercentage,
    List<LessonProgressDto> LessonProgress,
    List<SubmissionDto> Submissions,
    decimal? CourseRating,
    string Review
);
