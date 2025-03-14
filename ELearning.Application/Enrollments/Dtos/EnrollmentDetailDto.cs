using ELearning.Application.Submissions.Dtos;

namespace ELearning.Application.Enrollments.Dtos;

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
    string? Review
);